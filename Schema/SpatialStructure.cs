// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using global::System;
using global::System.Collections.Generic;
using global::Google.FlatBuffers;

public struct SpatialStructure : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static void ValidateVersion() { FlatBufferConstants.FLATBUFFERS_25_2_10(); }
  public static SpatialStructure GetRootAsSpatialStructure(ByteBuffer _bb) { return GetRootAsSpatialStructure(_bb, new SpatialStructure()); }
  public static SpatialStructure GetRootAsSpatialStructure(ByteBuffer _bb, SpatialStructure obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p = new Table(_i, _bb); }
  public SpatialStructure __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public uint? LocalId { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetUint(o + __p.bb_pos) : (uint?)null; } }
  public string Category { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetCategoryBytes() { return __p.__vector_as_span<byte>(6, 1); }
#else
  public ArraySegment<byte>? GetCategoryBytes() { return __p.__vector_as_arraysegment(6); }
#endif
  public byte[] GetCategoryArray() { return __p.__vector_as_array<byte>(6); }
  public SpatialStructure? Children(int j) { int o = __p.__offset(8); return o != 0 ? (SpatialStructure?)(new SpatialStructure()).__assign(__p.__indirect(__p.__vector(o) + j * 4), __p.bb) : null; }
  public int ChildrenLength { get { int o = __p.__offset(8); return o != 0 ? __p.__vector_len(o) : 0; } }

  public static Offset<SpatialStructure> CreateSpatialStructure(FlatBufferBuilder builder,
      uint? local_id = null,
      StringOffset categoryOffset = default(StringOffset),
      VectorOffset childrenOffset = default(VectorOffset)) {
    builder.StartTable(3);
    SpatialStructure.AddChildren(builder, childrenOffset);
    SpatialStructure.AddCategory(builder, categoryOffset);
    SpatialStructure.AddLocalId(builder, local_id);
    return SpatialStructure.EndSpatialStructure(builder);
  }

  public static void StartSpatialStructure(FlatBufferBuilder builder) { builder.StartTable(3); }
  public static void AddLocalId(FlatBufferBuilder builder, uint? localId) { builder.AddUint(0, localId); }
  public static void AddCategory(FlatBufferBuilder builder, StringOffset categoryOffset) { builder.AddOffset(1, categoryOffset.Value, 0); }
  public static void AddChildren(FlatBufferBuilder builder, VectorOffset childrenOffset) { builder.AddOffset(2, childrenOffset.Value, 0); }
  public static VectorOffset CreateChildrenVector(FlatBufferBuilder builder, Offset<SpatialStructure>[] data) { builder.StartVector(4, data.Length, 4); for (int i = data.Length - 1; i >= 0; i--) builder.AddOffset(data[i].Value); return builder.EndVector(); }
  public static VectorOffset CreateChildrenVectorBlock(FlatBufferBuilder builder, Offset<SpatialStructure>[] data) { builder.StartVector(4, data.Length, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateChildrenVectorBlock(FlatBufferBuilder builder, ArraySegment<Offset<SpatialStructure>> data) { builder.StartVector(4, data.Count, 4); builder.Add(data); return builder.EndVector(); }
  public static VectorOffset CreateChildrenVectorBlock(FlatBufferBuilder builder, IntPtr dataPtr, int sizeInBytes) { builder.StartVector(1, sizeInBytes, 1); builder.Add<Offset<SpatialStructure>>(dataPtr, sizeInBytes); return builder.EndVector(); }
  public static void StartChildrenVector(FlatBufferBuilder builder, int numElems) { builder.StartVector(4, numElems, 4); }
  public static Offset<SpatialStructure> EndSpatialStructure(FlatBufferBuilder builder) {
    int o = builder.EndTable();
    return new Offset<SpatialStructure>(o);
  }
}


static public class SpatialStructureVerify
{
  static public bool Verify(Google.FlatBuffers.Verifier verifier, uint tablePos)
  {
    return verifier.VerifyTableStart(tablePos)
      && verifier.VerifyField(tablePos, 4 /*LocalId*/, 4 /*uint*/, 4, false)
      && verifier.VerifyString(tablePos, 6 /*Category*/, false)
      && verifier.VerifyVectorOfTables(tablePos, 8 /*Children*/, SpatialStructureVerify.Verify, false)
      && verifier.VerifyTableEnd(tablePos);
  }
}
