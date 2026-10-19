"use client";

import Panel from "@/components/ui/Panel";
import SectionHeading from "@/components/ui/SectionHeading";
import ConditionFields, { type ConditionFieldsProps } from "@/components/pattern/ConditionFields";

/**
 * 条件のパネル(パターン検索 3.1)。
 * 夜の王と開始地点がそろうと、画面がすぐに検索する。検索のボタンは置かない。
 */
export default function ConditionPanel(props: ConditionFieldsProps) {
  return (
    <Panel ornate aria-labelledby="conditions-title">
      <SectionHeading eyebrow="Conditions" title="条件" id="conditions-title" />
      <ConditionFields {...props} />
    </Panel>
  );
}
