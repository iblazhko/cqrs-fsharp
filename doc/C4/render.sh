#!/usr/bin/env bash

# Render PlantUML files as PNG
#
# Prerequisites:
# * PlantUML (`brew install plantuml` / [download](https://plantuml.com/))

plantuml --version 1>/dev/null 2>&1
if [ $? -ne 0 ]; then
    echo "PlantUML could not be started"
    exit 1
fi

for x in $(ls *.puml); do
  FILE="${x%.*}"
  echo -n "Rendering ${FILE}.png ... "
  cat ${FILE}.puml | plantuml -charset UTF-8 -tpng -pipe > ${FILE}.png
  echo "done"
done
