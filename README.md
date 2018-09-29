# Cardknox API Wrapper for .NET Standard
API Wrapper for Cardknox Payment Processor written in C#, using .NET Standard 2.0.

Check out the latest release: [https://github.com/ahwm/Cardknox-API-Wrapper/releases](https://github.com/ahwm/Cardknox-API-Wrapper/releases)

[![NuGet Status](https://img.shields.io/badge/nuget-0.1.2--beta-brightgreen.svg)](https://www.nuget.org/packages/Cardknox.API.Wrapper/)

---

**Current Release:** [https://github.com/ahwm/Cardknox-API-Wrapper/releases/tag/v0.1.2](https://github.com/ahwm/Cardknox-API-Wrapper/releases/tag/v0.1.2)

CC Sale is live for testing.

---

API Documentation: [https://kb.cardknox.com/api](https://kb.cardknox.com/api)

# Progress
* CREDIT CARD: 47%
* CHECK (ACH): 0%
* EBT Food Stamp: 0%
* EBT Cash Benefits: 0%
* EBT Wic (eWic): 0%
* GIFT CARD: 0%
* FRAUD: 0%

## CREDIT CARD Progress
The base functionality is in place to make a credit card sale, but not all of the available fields are being included yet.

### Completed:

* cc:save
* cc:refund
* cc:void

### Partially Complete
Only required and recommended fields are included. This is unlikely to change in the near future. If you need to use more fields, fork it and add them yourself. I will gladly accept pull requests which add these optional fields.

* cc:sale
* cc:authonly
* cc:capture
* cc:credit