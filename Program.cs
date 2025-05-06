using System.Text;
using Google.FlatBuffers;

const string outputFile = "../../../materials.txt";
const string filePath = "../../../resources/AC20-FZK-Haus.frag";
var data = File.ReadAllBytes(filePath);

var bb = new ByteBuffer(data);

var model = Model.GetRootAsModel(bb);

ReadMeshes(model);

return;

static void ReadMeshes(Model model) {
    if (model.Meshes == null)
    {
        Console.WriteLine("No Meshes data found in the model.");
        return;
    }

    var sb = new StringBuilder();
    sb.AppendLine("Material Information Export");
    sb.AppendLine("==========================");
    sb.AppendLine();

    var materialsCount = model.Meshes.Value.MaterialsLength;
    sb.AppendLine($"Total Materials: {materialsCount}");
    sb.AppendLine();
    
    var accessMethods = new Dictionary<string, int>();
    
    sb.AppendLine("MATERIALS LIST");
    sb.AppendLine("==============");
    
    for (int i = 0; i < materialsCount; i++)
    {
        var materialInfo = MaterialDirectAccessor.GetMaterial(model, i);
        
        sb.AppendLine($"Material #{i + 1}:");
        sb.AppendLine($"  R: {materialInfo.R}");
        sb.AppendLine($"  G: {materialInfo.G}");
        sb.AppendLine($"  B: {materialInfo.B}");
        sb.AppendLine($"  A: {materialInfo.A}");
        sb.AppendLine($"  RenderedFaces: {materialInfo.RenderedFaces}");
        sb.AppendLine($"  Stroke: {materialInfo.Stroke}");
        sb.AppendLine($"  Access Method: {materialInfo.AccessMethod}");
        
        if (!accessMethods.TryGetValue(materialInfo.AccessMethod, out var count))
        {
            count = 0;
        }
        
        accessMethods[materialInfo.AccessMethod] = count + 1;
        
        sb.AppendLine();
    }
    
    foreach (var method in accessMethods)
    {
        sb.AppendLine($"{method.Key}: {method.Value} materials");
    }
    sb.AppendLine();
    
    try
    {
        File.WriteAllText(outputFile, sb.ToString());
        Console.WriteLine($"Material information successfully exported to: {outputFile}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error writing to file: {ex.Message}");
    }

}
