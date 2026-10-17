// 画面一覧のページの、試作の枠を、ページの幅に合わせて縮める。
// 中の画面は決まった幅で描き、枠の幅に収まるように全体を縮めて見せる。
// 枠の幅で描くと、本文の幅が狭いときに、広い画面の並びが狭い画面の並びに替わってしまうためである。
// 描く幅と高さは、枠の data-width と data-height で変えられる。既定は広い画面の 1,100 × 900 である。
(function () {
  "use strict";

  var DEFAULT_WIDTH = 1100;
  var DEFAULT_HEIGHT = 900;

  /** 枠を 1 つ、今の幅に合わせる */
  function fit(frame) {
    var iframe = frame.querySelector("iframe");
    if (!iframe) return;
    var width = Number(frame.dataset.width) || DEFAULT_WIDTH;
    var height = Number(frame.dataset.height) || DEFAULT_HEIGHT;
    var scale = Math.min(1, frame.clientWidth / width);
    iframe.style.width = width + "px";
    iframe.style.height = height + "px";
    iframe.style.transform = "scale(" + scale + ")";
    frame.style.height = Math.round(height * scale) + "px";
  }

  /** ページの中の枠を、すべて合わせる */
  function fitAll() {
    document.querySelectorAll(".screen-frame").forEach(fit);
  }

  window.addEventListener("resize", fitAll);
  // ページの移動を読み込みなしで行う設定でも動くように、Material の document$ があればそれに従う
  if (window.document$ && typeof window.document$.subscribe === "function") {
    window.document$.subscribe(fitAll);
  } else if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", fitAll);
  } else {
    fitAll();
  }
})();
