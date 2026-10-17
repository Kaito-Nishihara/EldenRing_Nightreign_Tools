import { describe, it, expect, vi, afterEach } from "vitest";
import { createId } from "@/lib/id";

const UUID_V4 = /^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/;

describe("createId", () => {
  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it("crypto.randomUUID があるときは、それで作る", () => {
    expect(createId()).toMatch(UUID_V4);
  });

  it("crypto.randomUUID がない(HTTPS でも localhost でもない)ときも、UUID の版 4 の形で作る", () => {
    vi.stubGlobal("crypto", { getRandomValues: crypto.getRandomValues.bind(crypto) });

    const ids = Array.from({ length: 50 }, () => createId());

    ids.forEach((id) => expect(id).toMatch(UUID_V4));
    expect(new Set(ids).size).toBe(50);
  });
});
