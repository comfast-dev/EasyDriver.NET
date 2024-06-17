# Changelog

[0.3.0]
- Reduce amount of types:
  - Remove `SelectorChain`
  - Join `SelectorParser` and `Xpath`
  - Differentiate methods child/main selector:
    - `_S()` child (renamed from `S()`)
    - `S()` main
  - extract `ElementFindFail` as separated Exception class