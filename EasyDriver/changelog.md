# Changelog

[0.2.4 IN PROGRESS]
- Fix WaitFor(throwIfFail = false)


[0.2.3]
- Reduce amount of types:
    - Remove `SelectorChain`
    - Join `SelectorParser` and `Xpath`
    - Differentiate methods child/main selector:
        - `_S()` child (renamed from `S()`)
        - `S()` main
    - extract `ElementFindFail` as separated Exception class