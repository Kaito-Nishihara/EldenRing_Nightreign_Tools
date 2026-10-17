"""一文ごとに改行して書いた日本語を、表示のときだけ 1 つの段落に詰める MkDocs のフック。

Markdown は段落の中の改行を空白として表示する。
日本語では文と文の間に余計な空白が入るので、句点などで終わる行と次の行をつなぐ。
元の Markdown は書き換えない。
"""

import re

# 行末がこの文字で、次の行が普通の文であるときにつなぐ
_SENTENCE_END = "。、」)!?"
# 次の行の先頭がこれらのときは、箇条書き、表、見出し、引用、番号付きの項目なのでつながない
_JOIN = re.compile(r"(?<=[" + _SENTENCE_END + r"])\n(?=[^\s\-*+|#>:`\d<!\[])")
_FENCE = re.compile(r"^(\s*)(```|~~~)")


def _join(text: str) -> str:
    return _JOIN.sub("", text)


def on_page_markdown(markdown: str, **kwargs) -> str:
    """コードブロックの外だけを対象に、行をつなぐ。"""
    out: list[str] = []
    buffer: list[str] = []
    in_fence = False
    for line in markdown.split("\n"):
        if _FENCE.match(line):
            if not in_fence:
                out.append(_join("\n".join(buffer)))
                buffer = []
            in_fence = not in_fence
            out.append(line)
            continue
        if in_fence:
            out.append(line)
        else:
            buffer.append(line)
    out.append(_join("\n".join(buffer)))
    return "\n".join(out)
