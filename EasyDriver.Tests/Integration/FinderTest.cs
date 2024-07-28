﻿using Comfast.EasyDriver.Se.Finder;
using EasyDriver.Tests.Integration.Infra;
using EasyDriver.Tests.Util;
using FluentAssertions;
using OpenQA.Selenium;
using Xunit.Abstractions;

namespace EasyDriver.Tests.Integration;

public class FinderTest : IntegrationBase {
    public FinderTest(ITestOutputHelper output, IntegrationFixture fix) : base(output, fix) { }

    private string _expectedText1 = "HTML with newlines:";
    private string _expectedText2 = "Hello World !";
    private string _css = "#textAndHtml td";
    private string _xpath = "//*[@id='textAndHtml']//td";
    private string _extendedCss = " body >> #textAndHtml td";
    private string _extendedXpath = "//body >> //*[@id='textAndHtml']//td";

    private readonly IWebDriver _driver = GetDriver();

    [Fact] void FinderFindTest() {
        ShouldFindPass(new WebElementFinder(_driver, _css).Find(), _expectedText1);
        ShouldFindPass(new WebElementFinder(_driver, _xpath).Find(), _expectedText1);
        ShouldFindPass(new WebElementFinder(_driver, _extendedCss).Find(), _expectedText1);
        ShouldFindPass(new WebElementFinder(_driver, _extendedXpath).Find(), _expectedText1);
    }

    [Fact] void FinderFindAllTest() {
        ShouldFindPass(new WebElementFinder(_driver, _css).FindAll()[1], _expectedText2);
        ShouldFindPass(new WebElementFinder(_driver, _xpath).FindAll()[1], _expectedText2);
        ShouldFindPass(new WebElementFinder(_driver, _extendedCss).FindAll()[1], _expectedText2);
        ShouldFindPass(new WebElementFinder(_driver, _extendedXpath).FindAll()[1], _expectedText2);

        // DoTest(selector => new WebElementFinder(driver, selector))
    }

    [Fact] void JsFinderFindTest() {
        ShouldFindPass(new JsFinder(_driver, _css).Find(), _expectedText1);
        ShouldFindPass(new JsFinder(_driver, _xpath).Find(), _expectedText1);
        ShouldFindPass(new JsFinder(_driver, _extendedCss).Find(), _expectedText1);
        ShouldFindPass(new JsFinder(_driver, _extendedXpath).Find(), _expectedText1);
    }

    [Fact] void JsFinderFindAllTest() {
        ShouldFindPass(new JsFinder(_driver, _css).FindAll()[1], _expectedText2);
        ShouldFindPass(new JsFinder(_driver, _xpath).FindAll()[1], _expectedText2);
        ShouldFindPass(new JsFinder(_driver, _extendedCss).FindAll()[1], _expectedText2);
        ShouldFindPass(new JsFinder(_driver, _extendedXpath).FindAll()[1], _expectedText2);
    }

    [Fact] void DriverExtensionsFind() {
        ShouldFindPass(_driver.Find(_css), _expectedText1);
        ShouldFindPass(_driver.Find(_xpath), _expectedText1);
        ShouldFindPass(_driver.Find(_extendedCss), _expectedText1);
        ShouldFindPass(_driver.Find(_extendedXpath), _expectedText1);
    }

    [Fact] void DriverExtensionsFindAll() {
        ShouldFindPass(_driver.FindAll(_css)[1], _expectedText2);
        ShouldFindPass(_driver.FindAll(_xpath)[1], _expectedText2);
        ShouldFindPass(_driver.FindAll(_extendedCss)[1], _expectedText2);
        ShouldFindPass(_driver.FindAll(_extendedXpath)[1], _expectedText2);
    }

    private void ShouldFindPass(IWebElement foundElement, string expectedText) {
        foundElement.Text.Should().Be(expectedText);
    }
}