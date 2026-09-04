#!/usr/bin/env bash
# Usage: ./build.sh

set -e

RIDS=("win-x64" "linux-x64" "linux-arm64" "osx-x64" "osx-arm64")
OUT_DIR="dist"

rm -rf "$OUT_DIR"

for rid in "${RIDS[@]}"; do
  echo "Building $rid..."
  dotnet publish Flux.CLI -c Release -r "$rid" -o "$OUT_DIR/$rid"

  cd "$OUT_DIR/$rid"
  if [[ "$rid" == win-* ]]; then
    zip -q "../flux-$rid.zip" flux.exe
  else
    chmod +x flux
    tar -czf "../flux-$rid.tar.gz" flux
  fi
  cd - > /dev/null
done

echo
echo "Done. Archives are in $OUT_DIR/"
