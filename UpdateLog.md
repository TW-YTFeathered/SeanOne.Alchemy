# Update Log

> ℹ️ This Update Log serves as a consolidated history of the project,  
> including versions developed in the original repository (now closed)  
> and all subsequent releases under the new name `SeanOne.Alchemy`.

## **Release**

## V2.0.0

*Focus:Rename*

- Renamed the project to `SeanOne.Alchemy`

### **Why the Rename to `SeanOne.Alchemy`?**

The project originally began as a simple object-to-string DSL formatter.
However, its long-term vision is broader than a single formatting tool.
The name **“Alchemy”** better reflects the project’s direction and future potential.

Although the current version focuses on:

* **object → string** formatting

the design philosophy behind the project aims to support a wider concept of **data transformation**, such as:

* object → object (flexible data shaping or mapping)
* string → object (config-driven parsing or construction)
* string → string (templating or programmable rewriting)

These future directions are *possibilities*, not promises, but they represent the conceptual scope that the name “Alchemy” is meant to capture.

Just as alchemy historically symbolized **transforming one substance into another**,
this project adopts the name to express its broader goal:
a general transformation framework driven by DSL and expressive rules.

`SeanOne.Alchemy` marks the beginning of that expanded vision.

## V1.1.0

*Focus:Fluent API*

- Added a fluent API to make DSL formatting simpler and more readable

### V1.0.1

- Corrected NuGet package URLs
- First published version on NuGet

### **V1.0.0**

*Focus:Initial release*

- Initial release of the DSL formatting tool

## **Beta**

### **beta 0.4.0**

*Focus:Bug fixs*

- Addressed errors arising from dependence on regular expressions or `string.Format()` by removing reliance on them
- Fixed the regular expression so that parameters are treated as valid matchable arguments, avoiding errors
- Converted function comments to a unified XML format
- Converted Markdown permalinks to relative URLs

### **beta 0.3.0**

*Focus:Refine GUIDE.md and improve API clarity*

- Added `GUIDE.md` to complement `README.md` with extended documentation
- Added asynchronous (`async`) methods to support non-blocking operations
- Renamed parameters to follow a more consistent and syntactically appropriate naming style, except for `tostring`, which directly invokes C#'s `ToString` method
- Replaced generic `throw` with specific exception type for clearer error semantics
- Improve the comments

### **beta 0.2.0**

*Focus:Bug fixs*

- Create a Git repository and push it to GitHub
- Renamed `namespace`, `class`, and `entry point` to align with `DSL` naming conventions
- Wrote Markdown files
- Refined the approach
- Fixed the issue where a parameter value was incorrectly interpreted as a parameter name
- Fixed the bug where `\r`, `\n`, etc. are incorrectly treated as whitespace

### **beta 0.1.0**

*Focus:Initial file creation*

- Extracted formatting module from an unpublished project and repurposed it as a DSL formatting tool
