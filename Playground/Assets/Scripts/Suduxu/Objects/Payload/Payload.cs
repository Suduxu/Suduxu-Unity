using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Payload
{
    public ushort id;
    public string name;
    public JToken values;

    public Payload(ushort id, string name, JToken values)
    {
        this.id = id;
        this.name = name;
        this.values = values;
    }

    public Payload(string name, JToken values)
    {
        this.name = name;
        this.values = values;
    }

    public override string ToString()
    {
        string valuesString = values != null
            ? values.ToString(Formatting.Indented)
            : "null";

        return
            $@"Payload
{{
    id: {id},
    name: ""{name}"",
    values: {valuesString}
}}";
    }
}