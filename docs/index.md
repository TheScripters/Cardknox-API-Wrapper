The purpose of this library is to fill a gap. Cardknox has decent documentation and code samples, but it requires every developer to implement every facet they use for every project. We started using Cardknox primarily as the payment provider at work and I wrote this for our internal projects but figured it would benefit the community at large.

The API is actually really simple to use and implement (compared to most), but this library should improve on that and make it even easier.

Check out the latest release: [https://github.com/TheScripters/Cardknox-API-Wrapper/releases/latest](https://github.com/TheScripters/Cardknox-API-Wrapper/releases/latest)

[![license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/ahwm/Cardknox-API-Wrapper/blob/master/LICENSE)
[![opened issues](https://img.shields.io/github/issues-raw/badges/shields/website.svg)](https://github.com/ahwm/Cardknox-API-Wrapper/issues)

## Nuget Package

[![NuGet Status](https://buildstats.info/nuget/Cardknox.API.Wrapper?includePreReleases=true)](https://www.nuget.org/packages/Cardknox.API.Wrapper/)

Available here: [https://www.nuget.org/packages/Cardknox.API.Wrapper/](https://www.nuget.org/packages/Cardknox.API.Wrapper/)

Install from Package Manager Console

```
PM> Install-Package Cardknox.API.Wrapper
```

---

API Documentation: [https://kb.cardknox.com/api](https://kb.cardknox.com/api)

## Progress

**Overall Completion: *100 %***

* CREDIT CARD: 100%
* CHECK (ACH): 100%
* EBT Food Stamp: 100%
* EBT Cash Benefits: 100%
* EBT Wic (eWic): 100%
* GIFT CARD: 100%
* FRAUD: 100%

All items have been completed, but hasn't yet been tested in a live environment. If you find it working in a production environment please let me know. I am only using `Credit Card` operations at this time, so nothing else has really been tested.
