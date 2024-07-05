# Changelog

[NOT RELEASED]
- [x] Validation of browser path and driver path, hints for user about download links
- [x] Optional (virtual) Description field in BaseComponent
- [x] Second parameter for description in `DriverApi.S("", "description")`
- [x] Change collections interface to from `ReadOnlyCollection` to `IList`
- [ ] Auto generating markdown docs
- [ ] Logger compatible with Xunit
- [ ] Auto download browser using selenium-manager
- [x] ConfigLoader bug: read simple types
- [x] Configuration Reload method with custom file e.g. MyAppConfig.json

### [0.2.6]
Fixed bugs:
1. Reconnect after Selenium version upgrade
2. WaitFor error message now contains selector.
3. Added new integration tests

### [0.2.5]
- Download files functionality

### [0.2.4]
- Refactored logic of BrowserRunner - add ability to customize it
- Fix WaitFor(throwIfFail = false)

### [0.2.3]
- Reduce amount of types:
    - Remove `SelectorChain`
    - Join `SelectorParser` and `Xpath`
    - Differentiate methods child/main selector:
        - `_S()` child (renamed from `S()`)
        - `S()` main
    - extract `ElementFindFail` as separated Exception class
