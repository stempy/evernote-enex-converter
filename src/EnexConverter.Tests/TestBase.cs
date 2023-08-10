using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Abstractions;

namespace EnexConverter.Tests;

public abstract class TestBase
{
    private readonly ITestOutputHelper _outputHelper;

    private readonly JsonSerializerOptions _serializerOptions = 
        new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() },
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };


    protected TestBase(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    protected void WriteJson(object model)
    {
        _outputHelper.WriteLine(JsonSerializer.Serialize(model,_serializerOptions));
    }

    protected void WriteLine(string line) 
        => _outputHelper.WriteLine(line);
}