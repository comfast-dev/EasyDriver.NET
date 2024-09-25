# Changelog

[TODO List]
- [ ] Refactor Description into structured NamesChain array (keep names from all parents)
- [x] Add configuration json's json-schema documentation
- [ ] Auto generating markdown docs
- [ ] Wrap every Action into "Action" to get:
  - [ ] Add ability to AutoWaiting
  - [ ] throw detailed ActionFailedException
  - [ ] Add ability to create action logs
  - [ ] Add ability to add before/after action hooks
  - [ ] Add ability to Measure action time/generate statistics/benchmarks
- [ ] Logger compatible with Xunit
- [ ] Auto download browser using selenium-manager
- [ ] Prepare Chromium browser flags for fastest run possible


### [0.4.2] - NOT RELEASED



### [0.4.1]
- Add Experimental find using JS
- Add Experimental actions using JS
- Rename HasClass -> HasCssClass
- Rearrange namespaces EasyDriver.Se -> EasyDriver.Core
- Rearrange namespaces EasyDriver.Ui -> EasyDriver.Lib


### [0.4.0]
CODE COVERAGE: 77%
BREAKING CHANGES:
- [x] Rename config file to EasyDriverConfig.json
- [x] Change DriverApi -> EasyDriverApi
- [x] Change AppConfig.json -> EasyDriverConfig.json
- [x] Change Configuration -> EasyDriverConfig
- [x] Change ILocator interface:
  - [x] Rename `S() -> GetLocator()`, `_S() -> SubLocator()` keeping aliases: `S`, `_S`
  - [x] Selector -> CssOrXpath
  - [x] FindElement -> FindWebElement
  - [x] IFoundLocator.FoundElement -> FoundWebElement

OTHER CHANGES:
- [x] Wrap every Action into "Action" method - for future improvements
- [x] Get Texts using Js experimental option
- [x] Add method: MapUsingJs
- [x] Fix IsDisplayed throwing StaleElementReference exception

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
