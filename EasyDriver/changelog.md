# Changelog

[TODO List]

- [x] Add configuration json's json-schema documentation
- [ ] Auto generating markdown docs
- [ ] Logger compatible with Xunit
- [ ] Auto download browser using selenium-manager
- [x] Get Texts using Js experimental option
- [x] Add method: MapUsingJs
- [x] Rename `S() -> Locator()`, `_S() -> SubLocator()` keeping aliases: `S`, `_S`
- [x] Fix IsDisplayed throwing StaleElementReference exception

### [0.4.x] - NOT RELEASED
BREAKING CHANGES:
- [x] Rename config file to EasyDriverConfig.json
- [x] Change API: FindElement, FindElements,
- [x] Change DriverApi -> EasyDriverApi
- [x] Change AppConfig.json -> EasyDriverConfig.json
- [x] Change Configuration -> EasyDriverConfig
- [ ] Change ILocator interface:
  - [ ] Selector -> LocatorString
    - [ ] Description -> NamesChain
    - [ ] LocatorName

### [0.3.1]
- [x] Add DriverConfig.WindowSize functionality

### [0.3.0]

- [x] Validation of browser path and driver path
- [x] Optional (virtual) Description field in BaseComponent
- [x] Second parameter for description in `DriverApi.S("", "description")`
- [x] Change collections interface to from `ReadOnlyCollection` to `IList`
- [x] ConfigLoader bug: read simple types
- [x] Configuration Reload method with custom file e.g. MyAppConfig.json
- [x] Configuration reload using IConfiguration
- [x] Multiple types of builds: Release parallel+headless+fast, Debug reconnect+headed
- [x] Added unit/integration tests coverage 76%

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
