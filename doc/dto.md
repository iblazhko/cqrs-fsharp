# Domain Transfer Objects

Domain may use a rich model that is not very well suited for transferring over
the wire or persistence.

Specifically for domain commands and events, data we send over the wire and
use in message bus or persistence (such as event streams) should be easily
(de-) serializable, which may not be the case for domain models in general
and languages such as F# that allow you to easily build rich domain models in
particular -- one example being discriminated unions -- but even simple enums
or date/time values on their own may cause issues with (de-) serialization.

To solve this problem we can use a *direct representation* of domain commands
and events that is technology-neutral and serialization-friendly, and *explicit
mapping* between this representation and the domain model. In the .NET world,
safest option to implement such a representation is
[POCO](https://en.wikipedia.org/wiki/Plain_old_CLR_object)s that only use
primitive CLR types, and mappers that use industry standard formats for
representing domain model values, e.g. strings in ISO 8601 format to represent
date or time values.

I am calling this *representation* of domain commands and events
***Domain Transfer Object*** (DTO).

Not to be confused with *Data Transfer Object* (also DTO) which serves slightly
different purpose. In the true spirit of DDD, *Domain Transfer Object* shares
abbreviation with *Data Transfer Object*, and somewhat similar to it, but means
something different.
