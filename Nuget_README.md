# SeanOne.Alchemy

A lightweight and efficient C# library for fast object-to-string transformation using a simple DSL syntax.

## Example Usage

```csharp
using SeanOne.Alchemy;

List<int> ints = Enumerable.Range(0, 10).ToList();

AlchemyFormatter.Format(ints, "fe /tostring:F2 /end:\\n /exclude-last-end:true");

/* Return:
0.00
1.00
2.00
3.00
4.00
5.00
6.00
7.00
8.00
9.00
*/
```

## Fluent API Example Usage

```csharp
using SeanOne.Alchemy.Builder;

List<int> ints = Enumerable.Range(0, 10).ToList();

AlchemyFormatBuilder.SelectFeSeq()
        .With(FeSeqParam.ToString, "F2")
        .With(FeSeqParam.End, "\\n")
        .With(FeSeqParam.ExcludeLastEnd, "true")
        .Build()
        .Run(ints);
/* Return:
0.00
1.00
2.00
3.00
4.00
5.00
6.00
7.00
8.00
9.00
*/
```

## Documentation

For complete documentation and more examples, see the [README.md](https://github.com/TW-YTFeathered/SeanOne.Alchemy/blob/V2.0.0/README.md) file.
For more code example, see the [GUIDE.md](https://github.com/TW-YTFeathered/SeanOne.Alchemy/blob/V2.0.0/GUIDE.md) file.

## GitHub Repository

[SeanOne.Alchemy GitHub](https://github.com/TW-YTFeathered/SeanOne.Alchemy/tree/V2.0.0)

## License

MIT License
