module CQRS.Domain.Moon

// Simplified set of moon phases; see https://moon.nasa.gov/moon-in-motion/moon-phases/ for the full list
type MoonPhase =
    | NewMoon
    | FirstQuarter
    | FullMoon
    | LastQuarter
