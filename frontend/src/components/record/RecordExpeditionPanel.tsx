"use client";

import type { ComponentProps } from "react";
import Panel from "@/components/ui/Panel";
import SectionHeading from "@/components/ui/SectionHeading";
import RecordConditionSummary from "@/components/record/RecordConditionSummary";
import RecordSaveControls from "@/components/record/RecordSaveControls";

interface RecordExpeditionPanelProps {
  condition: ComponentProps<typeof RecordConditionSummary>;
  save: ComponentProps<typeof RecordSaveControls>;
}

/**
 * 「今回の条件」のパネル(プレイ記録 3.1、3.5)。
 * 試作のとおり、条件と備考と保存を 1 つの飾り枠にまとめる。
 */
export default function RecordExpeditionPanel({ condition, save }: RecordExpeditionPanelProps) {
  return (
    <Panel ornate aria-labelledby="record-expedition-title">
      <SectionHeading eyebrow="This Expedition" title="今回の条件" id="record-expedition-title" />
      <div className="flex flex-col gap-4">
        <RecordConditionSummary {...condition} />
        <RecordSaveControls {...save} />
      </div>
    </Panel>
  );
}
