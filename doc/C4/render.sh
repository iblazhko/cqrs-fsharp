#!/usr/bin/env bash

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
# * PlantUML (`brew install plantuml` / [download](https://plantuml.com/))
# * Inkscape (`flatpak install org.inkscape.Inkscape` / [download](https://inkscape.org/))

if [[ "$OSTYPE" == "linux"* ]]; then
    INKSCAPE=/var/lib/flatpak/exports/bin/org.inkscape.Inkscape
elif [[ "$OSTYPE" == "darwin"* ]]; then
    INKSCAPE=/Applications/Inkscape.app/Contents/MacOS/inkscape
else
    echo "OS $OSTYPE is not supported"
    exit 1
fi

plantuml --version 1>/dev/null 2>&1
if [ $? -ne 0 ]; then
    echo "PlantUML could not be starfted"
    exit 1
fi

if [ ! -f "$INKSCAPE" ]; then
    echo "Inkscape not found at $INKSCAPE"
    exit 1
fi

render_puml () {
  FILE=$1
  PNG_WIDTH=$2

  echo $FILE
  cat ${FILE}.puml | plantuml -charset UTF-8 -tsvg -pipe > ${FILE}.svg
  $INKSCAPE -w $PNG_WIDTH ${FILE}.svg -o ${FILE}.png
}

render_puml context 800
render_puml container 1600
render_puml component 1600
render_puml code-cmd-handler 2500
