// 見た目の試作の共通の処理。
// ヘッダーを差し込み、マップを SVG で描き、押したときの切り替えだけを動かす。
// API は呼ばない。押しても保存は起きない。
// マップ画像と開始地点のアイコンは、アプリと同じ frontend/public の画像を使う。点の座標は旧マイグレーション(V7、V8、V10)の値である。
// 第二収縮地点だけは、収縮候補地の中心と重ならないよう、試作のために中心からずらしている。
(function () {
  "use strict";

  var SVG_NS = "http://www.w3.org/2000/svg";

  /**
   * frontend/public の置き場。ファイルとして直接開いたときはリポジトリの中を、
   * 仕様書のサイトから開いたときは、サイトが写した frontend-public を指す(docs-site/hooks/copy_frontend_public.py)。
   */
  var ASSET_BASE = location.protocol === "file:" ? "../../frontend/public/" : "../frontend-public/";

  /** マップの識別子と画像。地変の識別子はデータベースの共通仕様 5.6 のとおり */
  var MAP_IMAGES = {
    normal: "Default.webp",
    crater: "jihen-kakou.webp",
    mountain: "jihen-sankan.webp",
    rotted_forest: "jihen-fuhai.webp",
    noklateo: "jihen-nokurateou.webp",
  };

  /** マスタの試作データ(マップ画像の 1024 px 基準の座標) */
  var DATA = {
    spawns: [
      ["北東の教会", 351, 387], ["西の教会", 216, 578], ["南西の教会", 218, 763],
      ["南の教会", 577, 830], ["南東の教会", 561, 679], ["北の教会", 547, 215],
      ["北西の教会", 218, 360], ["東の教会", 697, 398], ["東の野営地", 808, 569],
    ],
    // 収縮候補地。試作では第二収縮地点と同じ場所を中心にし、半径を 255(旧版のデータベースで利用者が登録した値)にしている
    zones: [
      ["最北西", 219, 233], ["中央西", 342, 488], ["最北東", 789, 248], ["最北", 583, 297],
      ["最東", 739, 518], ["中央東", 558, 552], ["最南東", 715, 692], ["最南", 493, 774],
      ["中央南西", 383, 711], ["最南西", 277, 728], ["最西", 211, 402], ["中央南", 408, 624],
    ],
    bases: [
      [404, 798, "?"], [615, 698, "砦"], [279, 667, "?"], [317, 562, "教"], [278, 459, "?"],
      [225, 272, "遺"], [410, 197, "?"], [434, 316, "?"], [638, 301, "塔"], [742, 232, "?"],
      [801, 438, "?"], [575, 451, "?"], [677, 501, "野"], [766, 763, "?"], [777, 636, "?"],
    ],
    zoneRadius: 255,
  };

  /** 第二収縮地点。試作では、同じ名前の収縮候補地の中心から円の中へずらした位置に置く */
  var POINT_OFFSETS = [[40, 50], [-50, 30], [-30, 60], [45, -35], [-55, -20], [30, 55], [-40, -45], [50, 20], [-25, -55], [55, -30], [35, 45], [-45, 35]];
  DATA.points = DATA.zones.map(function (z, i) {
    return [z[0], z[1] + POINT_OFFSETS[i][0], z[2] + POINT_OFFSETS[i][1]];
  });

  /** パターンの色(Paul Tol の 8 色) */
  var PATTERN_COLORS = ["#4477AA", "#EE6677", "#228833", "#CCBB44", "#66CCEE", "#AA3377", "#BBBBBB", "#EE7733"];

  var body = document.body;

  /* ---------- ヘッダー ---------- */

  function headerHtml() {
    var cur = body.dataset.current || "";
    function link(href, name, label) {
      return '<a href="' + href + '"' + (cur === name ? ' aria-current="page"' : "") + ">" + label + "</a>";
    }
    var right = cur === "admin"
      ? '<span class="admin-tools"><span class="admin-flag">ADMIN</span>' +
        '<button class="btn btn--ghost t-sm" type="button">鍵を消す</button></span>'
      : '<nav class="nav" aria-label="画面">' +
        link("pattern.html", "pattern", "パターン") +
        link("record.html", "record", "記録する") +
        link("build.html", "build", "ビルド") +
        "</nav>";
    return '<a class="brand" href="pattern.html"><span class="brand__mark">NIGHTREIGN</span>' +
      '<span class="brand__name">収縮パターン帳</span></a>' + right;
  }

  function insertHeader() {
    var header = document.createElement("header");
    header.className = "site-header";
    header.innerHTML = headerHtml();
    body.insertBefore(header, body.firstChild);
  }

  /* ---------- マップ ---------- */

  function el(name, attrs, parent) {
    var node = document.createElementNS(SVG_NS, name);
    Object.keys(attrs || {}).forEach(function (k) { node.setAttribute(k, attrs[k]); });
    if (parent) parent.appendChild(node);
    return node;
  }

  /**
   * マップ画像を敷く。画像の大きさは 1000 px 四方だが、座標は 1024 の座標空間で持つので、1024 に引き伸ばして描く
   * (画面の共通仕様 3.8)。
   */
  function drawMapImage(svg, mapKey) {
    var file = MAP_IMAGES[mapKey || "normal"] || MAP_IMAGES.normal;
    el("rect", { x: 0, y: 0, width: 1024, height: 1024, fill: "#07060a" }, svg);
    el("image", { href: ASSET_BASE + "map/" + file, x: 0, y: 0, width: 1024, height: 1024, preserveAspectRatio: "none" }, svg);
    // 明るい地図を、画面の暗い配色になじませる薄い影
    el("rect", { x: 0, y: 0, width: 1024, height: 1024, class: "m-map-shade" }, svg);
  }

  var maskSeq = 0;

  /**
   * 収縮の円の外を、夜の雨の暗がりで塗る。円の中は動ける場所なので塗らない。
   * 円が 2 つ(「すべて」の日)のときは、どちらの円にも入らない所を塗る。縁はぼかして、外へ向けて暗くする。
   */
  function drawRain(svg, zoneIndices) {
    var zones = zoneIndices.map(function (i) { return DATA.zones[i]; }).filter(Boolean);
    if (zones.length === 0) return;
    var id = "rain-mask-" + (++maskSeq);
    var defs = el("defs", {}, svg);
    var blur = el("filter", { id: id + "-blur", x: "-10%", y: "-10%", width: "120%", height: "120%" }, defs);
    el("feGaussianBlur", { stdDeviation: 10 }, blur);
    var mask = el("mask", { id: id, maskUnits: "userSpaceOnUse", x: 0, y: 0, width: 1024, height: 1024 }, defs);
    el("rect", { x: 0, y: 0, width: 1024, height: 1024, fill: "#fff" }, mask);
    zones.forEach(function (z) {
      el("circle", { cx: z[1], cy: z[2], r: DATA.zoneRadius, fill: "#000", filter: "url(#" + id + "-blur)" }, mask);
    });
    el("rect", { x: 0, y: 0, width: 1024, height: 1024, class: "m-rain", mask: "url(#" + id + ")" }, svg);
  }

  /**
   * 収縮候補地。12 の円をすべて実際の大きさで描くと重なって押せないので、中心の印と名前で出す。
   * 選んだものだけを、実際の大きさの円で描く。muted のときは印を薄くし、押せなくする(第 2 次を選んでいるとき)。
   */
  function drawZones(svg, selected, onPick, muted) {
    selected.forEach(function (i) {
      var z = DATA.zones[i];
      if (z) el("circle", { cx: z[1], cy: z[2], r: DATA.zoneRadius, class: "m-zone-circle" }, svg);
    });
    DATA.zones.forEach(function (z, i) {
      var isSelected = selected.indexOf(i) >= 0;
      var g = el("g", { class: "m-zone" + (isSelected ? " is-selected" : "") + (muted ? " is-muted" : "") }, svg);
      var c = el("circle", { cx: z[1], cy: z[2], r: 16, class: "m-zone-mark", tabindex: -1 }, g);
      var title = el("title", {}, c);
      title.textContent = z[0] + "(収縮候補地)";
      if (onPick && !muted) g.addEventListener("click", function () { onPick(i); });
      if (muted && !isSelected) return;
      var label = el("text", { x: z[1], y: z[2] - 26, class: "m-zone-label" }, g);
      label.textContent = z[0];
    });
  }

  /** 第二収縮地点。focusZone の円の外の地点は薄く出すが、押せるままにする */
  function drawPoints(svg, onPick, focusZone) {
    var focus = DATA.zones[focusZone];
    DATA.points.forEach(function (p, i) {
      var inside = !focus || Math.hypot(p[1] - focus[1], p[2] - focus[2]) <= DATA.zoneRadius;
      var size = onPick ? 22 : 14;
      var g = el("g", { opacity: inside ? 1 : 0.4 }, svg);
      var r = el("rect", {
        x: p[1] - size / 2, y: p[2] - size / 2, width: size, height: size,
        transform: "rotate(45 " + p[1] + " " + p[2] + ")",
        class: "m-point" + (onPick ? " is-pickable" : ""),
      }, g);
      var title = el("title", {}, r);
      title.textContent = p[0] + "(第二収縮地点)";
      if (onPick) {
        r.addEventListener("click", function () { onPick(i); });
        var label = el("text", { x: p[1], y: p[2] + 30, class: "m-zone-label m-point-label" }, g);
        label.textContent = p[0];
      }
    });
  }

  /** 開始地点。アプリと同じ spawn.webp を置き、選んだものは大きくして青の輪で囲む */
  function drawSpawns(svg, selected) {
    DATA.spawns.forEach(function (s, i) {
      var isSelected = selected === i;
      var size = isSelected ? 56 : 40;
      var g = el("g", { opacity: selected >= 0 && !isSelected ? 0.45 : 1 }, svg);
      if (isSelected) el("circle", { cx: s[1], cy: s[2], r: size / 2 + 4, class: "m-spawn-ring" }, g);
      el("image", { href: ASSET_BASE + "icons/zones/spawn.webp", x: s[1] - size / 2, y: s[2] - size / 2, width: size, height: size }, g);
      var title = el("title", {}, g);
      title.textContent = s[0];
    });
  }

  function drawBases(svg) {
    DATA.bases.forEach(function (b) {
      el("circle", { cx: b[0], cy: b[1], r: 16, class: "m-base" }, svg);
      var t = el("text", { x: b[0], y: b[1], class: "m-base-label" }, svg);
      t.textContent = b[2];
    });
  }

  /**
   * パターン検索のマップ。選んだパターンの実際の収縮先だけを描き、収縮候補地の円は描かない。
   * pattern は日ごとの第 1 次の収縮候補地、second は日ごとの第 2 次の地点(なければ null)。
   * day が "1" か "2" のときは、その日だけを描き、円の外を夜の雨の暗がりで塗る。
   */
  function drawPatternActual(svg, pattern, second, index, spawn, day) {
    if (!pattern) return;
    var color = PATTERN_COLORS[index % PATTERN_COLORS.length];
    var days = day === "1" ? [0] : day === "2" ? [1] : [0, 1];
    drawRain(svg, days.map(function (k) { return pattern[k]; }));

    // 「すべて」のときだけ、開始地点から日の順に第 1 次の中心をつなぐ
    if (days.length === 2) {
      var s = DATA.spawns[spawn] || DATA.spawns[0];
      var pts = [[s[1], s[2]]].concat(pattern.map(function (zi) { return [DATA.zones[zi][1], DATA.zones[zi][2]]; }));
      var d = pts.map(function (pt, k) { return (k === 0 ? "M" : "L") + pt[0] + " " + pt[1]; }).join(" ");
      el("path", { d: d, class: "m-route", stroke: color }, svg);
    }

    days.forEach(function (k) {
      var z = DATA.zones[pattern[k]];
      var dayLabel = k === 0 ? "I" : "II";
      el("circle", { cx: z[1], cy: z[2], r: DATA.zoneRadius, class: "m-actual", stroke: color }, svg);
      var name = el("text", { x: z[1], y: z[2] - DATA.zoneRadius + 26, class: "m-zone-label" }, svg);
      name.textContent = "DAY " + dayLabel + " " + z[0];
      var pi = second ? second[k] : null;
      if (pi !== null && pi !== undefined) {
        var pt = DATA.points[pi];
        el("line", { x1: z[1], y1: z[2], x2: pt[1], y2: pt[2], stroke: color, "stroke-width": 2, "stroke-dasharray": "6 5" }, svg);
        el("rect", { x: pt[1] - 10, y: pt[2] - 10, width: 20, height: 20, transform: "rotate(45 " + pt[1] + " " + pt[2] + ")", fill: color, stroke: "#0e0c0a", "stroke-width": 2 }, svg);
      }
      el("circle", { cx: z[1], cy: z[2], r: 18, fill: color, stroke: "#0e0c0a", "stroke-width": 2 }, svg);
      var t = el("text", { x: z[1], y: z[2], class: "m-chip-text" }, svg);
      t.textContent = dayLabel;
    });
  }

  function parseList(value) {
    return value ? JSON.parse(value) : [];
  }

  /** data 属性を読んで、マップを描き直す */
  function renderMap(host) {
    host.innerHTML = "";
    var svg = el("svg", { viewBox: "0 0 1024 1024", preserveAspectRatio: "xMidYMid meet", role: "img", "aria-label": host.dataset.label || "マップ" }, null);
    host.appendChild(svg);
    var layers = (host.dataset.layers || "").split(",");
    var selectedZones = parseList(host.dataset.selectedZones);
    var spawn = host.dataset.spawn ? Number(host.dataset.spawn) : -1;

    // 記録の画面では、選んでいるスロットの段階で、押せるものを替える(zones: 収縮候補地、points: 第二収縮地点)
    var mode = host.dataset.mode || "";
    function pick(kind) {
      return function (i) { host.dispatchEvent(new CustomEvent("mappick", { detail: { kind: kind, index: i } })); };
    }

    drawMapImage(svg, host.dataset.map);
    if (host.dataset.rain) drawRain(svg, [Number(host.dataset.rain)]);
    if (layers.indexOf("zones") >= 0) {
      drawZones(svg, selectedZones, host.dataset.pickable ? pick("zone") : null, mode === "points");
    }
    if (layers.indexOf("points") >= 0 && mode !== "zones") {
      drawPoints(svg, host.dataset.pickable && mode === "points" ? pick("point") : null, host.dataset.rain ? Number(host.dataset.rain) : -1);
    }
    if (layers.indexOf("bases") >= 0) drawBases(svg);
    if (layers.indexOf("pattern") >= 0) {
      var active = host.dataset.active ? Number(host.dataset.active) : 0;
      drawPatternActual(svg, parseList(host.dataset.patterns)[active], parseList(host.dataset.seconds)[active], active, spawn, host.dataset.day || "all");
    }
    if (layers.indexOf("spawn") >= 0) drawSpawns(svg, spawn);
    if (host.dataset.preview) {
      var p = host.dataset.preview.split(",").map(Number);
      el("circle", { cx: p[0], cy: p[1], r: p[2], class: "m-preview" }, svg);
      el("line", { x1: p[0] - 14, y1: p[1], x2: p[0] + 14, y2: p[1], class: "m-cross" }, svg);
      el("line", { x1: p[0], y1: p[1] - 14, x2: p[0], y2: p[1] + 14, class: "m-cross" }, svg);
    }
  }

  /* ---------- 切り替え ---------- */

  /** 同じ組の中で 1 つだけを押した状態にする */
  function bindToggleGroups() {
    document.querySelectorAll("[data-toggle-group]").forEach(function (group) {
      group.addEventListener("click", function (ev) {
        var btn = ev.target.closest("button");
        if (!btn || !group.contains(btn)) return;
        group.querySelectorAll("button").forEach(function (b) {
          if (b.closest("[data-toggle-group]") === group) b.setAttribute("aria-pressed", String(b === btn));
        });
      });
    });
  }

  /** パターンのカードを押すと、マップをそのパターンの収縮先に替える。日の切り替えで、描く日を絞る */
  function bindPatterns() {
    var map = document.querySelector(".mock-map[data-patterns]");
    if (!map) return;
    document.querySelectorAll("[data-pattern]").forEach(function (btn) {
      btn.addEventListener("click", function () {
        map.dataset.active = btn.dataset.pattern;
        renderMap(map);
      });
    });
    document.querySelectorAll("[data-day-filter] button").forEach(function (btn) {
      btn.addEventListener("click", function () {
        map.dataset.day = btn.dataset.day;
        renderMap(map);
      });
    });
  }

  /**
   * 記録の画面。第 1 次のスロットでは収縮候補地、第 2 次のスロットでは第二収縮地点を押して入れ、
   * 次の空いたスロットへ移る。マップは、選んでいるスロットの日と段階に合わせて描き直す。
   */
  function bindRecord() {
    var map = document.querySelector(".mock-map[data-pickable]");
    var slots = Array.prototype.slice.call(document.querySelectorAll(".slot"));
    if (!map || slots.length === 0) return;
    var count = document.querySelector("[data-filled-count]");
    var handle = document.querySelector(".sheet-handle");
    var dayButtons = document.querySelectorAll("[data-day-toggle] button");

    function activeSlot() {
      return slots.filter(function (s) { return s.getAttribute("aria-pressed") === "true"; })[0];
    }

    /** 選んでいるスロットに合わせて、マップの押せるもの、暗がり、日の切り替えを直す */
    function syncMap() {
      var active = activeSlot();
      if (!active) {
        map.dataset.mode = "";
        renderMap(map);
        if (handle) handle.textContent = "すべて入力済み ／ 開く";
        return;
      }
      var day = active.dataset.day;
      var first = slots.filter(function (s) { return s.dataset.day === day && s.dataset.phase === "1"; })[0];
      var zone = first ? first.dataset.zone : "";
      map.dataset.mode = active.dataset.phase === "2" ? "points" : "zones";
      if (zone) {
        map.dataset.rain = zone;
        map.dataset.selectedZones = "[" + zone + "]";
      } else {
        delete map.dataset.rain;
        map.dataset.selectedZones = "[]";
      }
      dayButtons.forEach(function (b, i) { b.setAttribute("aria-pressed", String(String(i + 1) === day)); });
      renderMap(map);
      if (handle) {
        handle.textContent = "DAY " + (day === "1" ? "I" : "II") + " 第 " + active.dataset.phase + " 次を入力中 ／ 開く";
      }
    }

    map.addEventListener("mappick", function (ev) {
      var active = activeSlot();
      if (!active) return;
      var isPoint = ev.detail.kind === "point";
      if (isPoint !== (active.dataset.phase === "2")) return;
      var name = (isPoint ? DATA.points : DATA.zones)[ev.detail.index][0];
      var value = active.querySelector(".slot__value");
      value.textContent = name;
      value.classList.remove("slot__value--empty");
      active.querySelector(".slot__state").textContent = "✓";
      if (!isPoint) active.dataset.zone = String(ev.detail.index);
      var next = slots.filter(function (s) { return s.querySelector(".slot__value--empty"); })[0];
      slots.forEach(function (s) { s.setAttribute("aria-pressed", String(s === next)); });
      if (count) count.textContent = String(slots.length - slots.filter(function (s) { return s.querySelector(".slot__value--empty"); }).length);
      syncMap();
    });

    // スロットを押して選び直したときも、マップを合わせる。DAY I と DAY II の 2 つの組をまたいで 1 つだけを選ぶ
    slots.forEach(function (s) {
      s.addEventListener("click", function () {
        slots.forEach(function (o) { o.setAttribute("aria-pressed", String(o === s)); });
        syncMap();
      });
    });
  }

  function bindSheet() {
    document.querySelectorAll(".sheet-handle").forEach(function (h) {
      h.addEventListener("click", function () {
        var side = h.closest(".layout__side");
        var collapsed = side.classList.toggle("is-collapsed");
        h.setAttribute("aria-expanded", String(!collapsed));
      });
    });
  }

  insertHeader();
  document.querySelectorAll(".mock-map").forEach(renderMap);
  bindToggleGroups();
  bindPatterns();
  bindRecord();
  bindSheet();
})();
