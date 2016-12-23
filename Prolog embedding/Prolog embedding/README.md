# Prolog Embedding in C#

## Assumptions

The following aspects are assumed in this implementation of Prolog:
* In case an term or clause in not in the knowledge base, it is considered false.
* The conjunction operator is not used, as this results in the same behaviour als individual rules and from a functional perspective this makes its implementation redundant.
