#!/usr/bin/bash

# Render PlantUML files as PNG
#
# PlantUML PNG rendering produces noticeably blurry images.
# To have better rendering, diagrams here redered as SVG first,
# then converted to PNG using Inkscape that produces better
# results in this particular case.
#
# This also avoids a problem with PNG images exceeding default
# PlantUML limit (4096px); if this happens, PNG rendered by
# PlantUML is cropped to the maximum size and parts of diagram
# may be missing from final rendering. Workaround is to set
# PLANTUML_LIMIT_SIZE environment variable to a large enough
# value but it is not necessary when rendering SVG.
#
# Prerequisites:
# * PlantUML (`brew install plantuml`)
# * Inkscape (`flatpak install org.inkscape.Inkscape`)

INKSCAPE=/var/lib/flatpak/exports/bin/org.inkscape.Inkscape

render_puml () {
  FILE=$1
  PNG_WIDTH=$2

  echo $FILE
  cat ${FILE}.puml | plantuml -tsvg -pipe > ${FILE}.svg
  $INKSCAPE -w $PNG_WIDTH ${FILE}.svg -o ${FILE}.png
}

render_puml context 800
render_puml container 1600
