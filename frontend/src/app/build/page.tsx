"use client";

import Button from "@/components/ui/Button";
import Panel from "@/components/ui/Panel";
import SectionHeading from "@/components/ui/SectionHeading";
import { ErrorNotice, Skeleton } from "@/components/ui/StatusViews";
import BuildSlotColumn from "@/components/build/BuildSlotColumn";
import BuildTotalsPanel from "@/components/build/BuildTotalsPanel";
import { useBuildSlots } from "@/hooks/useBuildSlots";
import { useRelicEffects } from "@/hooks/useRelicEffects";
import { SLOTS_PER_KIND, type BuildSlot } from "@/lib/buildCalc";

const KINDS: BuildSlot["kind"][] = ["normal", "deep"];

/** 読み込み中の 6 枠分の骨組み(ビルドシミュレーター 3.3) */
function SlotSkeletons() {
  return (
    <>
      {KINDS.map((kind) => (
        <Panel key={kind} aria-busy="true" aria-label="読み込み中">
          <Skeleton className="mb-4 h-10 w-32" />
          <div className="flex flex-col gap-3">
            {Array.from({ length: SLOTS_PER_KIND }, (_, i) => (
              <Skeleton key={i} className="h-24" />
            ))}
          </div>
        </Panel>
      ))}
    </>
  );
}

/**
 * ビルドシミュレーター(ビルドシミュレーター 3 章)。
 * 広い幅では通常効果、深層効果、効果の合計の 3 列、狭い幅では縦に並べる。
 * 効果のマスタは開いたときに 1 回だけ取り、合計はブラウザで計算する(決定の記録 0006)。
 */
export default function BuildPage() {
  const relic = useRelicEffects();
  const build = useBuildSlots(relic.effectMap);

  return (
    <main className="mx-auto grid max-w-[1200px] items-start gap-4 px-4 pb-12 pt-6 lg:grid-cols-[1fr_1fr_340px]">
      <div className="flex flex-col items-start justify-between gap-2 sm:flex-row sm:items-end lg:col-span-3">
        <SectionHeading level="h1" eyebrow="Relic Rite" title="ビルドシミュレーター" />
        {/* 保存していないので失うものが少なく、確かめの枠は出さない(3.1) */}
        <Button variant="danger" onClick={build.clearAll} disabled={relic.loading || relic.error !== null}>
          すべて外す
        </Button>
      </div>

      {relic.error ? (
        <div className="lg:col-span-3">
          <ErrorNotice message={relic.error} onRetry={relic.retry} />
        </div>
      ) : relic.loading ? (
        <SlotSkeletons />
      ) : (
        KINDS.map((kind) => (
          <BuildSlotColumn
            key={kind}
            kind={kind}
            slots={build.slots.filter((slot) => slot.kind === kind)}
            options={kind === "deep" ? relic.deepOptions : relic.normalOptions}
            effectMap={relic.effectMap}
            onEffectChange={build.chooseEffect}
            onRankChange={build.chooseRank}
          />
        ))
      )}

      {!relic.error && <BuildTotalsPanel totals={build.totals} />}
    </main>
  );
}
