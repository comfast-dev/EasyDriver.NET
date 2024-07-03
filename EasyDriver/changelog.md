# Changelog

[0.2.6]
Fixed bugs:
1. Reconnect after Selenium version upgrade
2. WaitFor error message now contains selector.
3. Added new integration tests

[0.2.5]
- Download files functionality

[0.2.4]
- Refactored logic of BrowserRunner - add ability to customize it
- Fix WaitFor(throwIfFail = false)

[0.2.3]
- Reduce amount of types:
    - Remove `SelectorChain`
    - Join `SelectorParser` and `Xpath`
    - Differentiate methods child/main selector:
        - `_S()` child (renamed from `S()`)
        - `S()` main
    - extract `ElementFindFail` as separated Exception class