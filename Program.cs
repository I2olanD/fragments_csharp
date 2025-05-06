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
    
    sb.AppendLine("MATERIALS LIST");
    sb.AppendLine("==============");
    
    for (var i = 0; i < materialsCount; i++)
    {
        var material = model.Meshes.Value.Materials(i);

        if (material == null)
        {
            return;
        }
        
        sb.AppendLine($"Material #{i + 1}:");
        sb.AppendLine($"  R: {material.Value.R}");
        sb.AppendLine($"  G: {material.Value.G}");
        sb.AppendLine($"  B: {material.Value.B}");
        sb.AppendLine($"  A: {material.Value.A}");
        sb.AppendLine($"  RenderedFaces: {material.Value.RenderedFaces}");
        sb.AppendLine($"  Stroke: {material.Value.Stroke}");
        
        sb.AppendLine();
    }
    
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
