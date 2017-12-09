// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: FileTransfer.proto
#pragma warning disable 1591, 0612, 3021
#region Designer generated code

using pb = global::Google.Protobuf;
using pbc = global::Google.Protobuf.Collections;
using pbr = global::Google.Protobuf.Reflection;
using scg = global::System.Collections.Generic;
namespace Message {

  /// <summary>Holder for reflection information generated from FileTransfer.proto</summary>
  public static partial class FileTransferReflection {

    #region Descriptor
    /// <summary>File descriptor for FileTransfer.proto</summary>
    public static pbr::FileDescriptor Descriptor {
      get { return descriptor; }
    }
    private static pbr::FileDescriptor descriptor;

    static FileTransferReflection() {
      byte[] descriptorData = global::System.Convert.FromBase64String(
          string.Concat(
            "ChJGaWxlVHJhbnNmZXIucHJvdG8SB01lc3NhZ2UiNQoMRmlsZVRyYW5zZmVy",
            "EhQKDGZpbGVDaGVja3N1bRgBIAEoCRIPCgdwYXlsb2FkGAIgASgJYgZwcm90",
            "bzM="));
      descriptor = pbr::FileDescriptor.FromGeneratedCode(descriptorData,
          new pbr::FileDescriptor[] { },
          new pbr::GeneratedClrTypeInfo(null, new pbr::GeneratedClrTypeInfo[] {
            new pbr::GeneratedClrTypeInfo(typeof(global::Message.FileTransfer), global::Message.FileTransfer.Parser, new[]{ "FileChecksum", "Payload" }, null, null, null)
          }));
    }
    #endregion

  }
  #region Messages
  public sealed partial class FileTransfer : pb::IMessage<FileTransfer> {
    private static readonly pb::MessageParser<FileTransfer> _parser = new pb::MessageParser<FileTransfer>(() => new FileTransfer());
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pb::MessageParser<FileTransfer> Parser { get { return _parser; } }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public static pbr::MessageDescriptor Descriptor {
      get { return global::Message.FileTransferReflection.Descriptor.MessageTypes[0]; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    pbr::MessageDescriptor pb::IMessage.Descriptor {
      get { return Descriptor; }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FileTransfer() {
      OnConstruction();
    }

    partial void OnConstruction();

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FileTransfer(FileTransfer other) : this() {
      fileChecksum_ = other.fileChecksum_;
      payload_ = other.payload_;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public FileTransfer Clone() {
      return new FileTransfer(this);
    }

    /// <summary>Field number for the "fileChecksum" field.</summary>
    public const int FileChecksumFieldNumber = 1;
    private string fileChecksum_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string FileChecksum {
      get { return fileChecksum_; }
      set {
        fileChecksum_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    /// <summary>Field number for the "payload" field.</summary>
    public const int PayloadFieldNumber = 2;
    private string payload_ = "";
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public string Payload {
      get { return payload_; }
      set {
        payload_ = pb::ProtoPreconditions.CheckNotNull(value, "value");
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override bool Equals(object other) {
      return Equals(other as FileTransfer);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public bool Equals(FileTransfer other) {
      if (ReferenceEquals(other, null)) {
        return false;
      }
      if (ReferenceEquals(other, this)) {
        return true;
      }
      if (FileChecksum != other.FileChecksum) return false;
      if (Payload != other.Payload) return false;
      return true;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override int GetHashCode() {
      int hash = 1;
      if (FileChecksum.Length != 0) hash ^= FileChecksum.GetHashCode();
      if (Payload.Length != 0) hash ^= Payload.GetHashCode();
      return hash;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public override string ToString() {
      return pb::JsonFormatter.ToDiagnosticString(this);
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void WriteTo(pb::CodedOutputStream output) {
      if (FileChecksum.Length != 0) {
        output.WriteRawTag(10);
        output.WriteString(FileChecksum);
      }
      if (Payload.Length != 0) {
        output.WriteRawTag(18);
        output.WriteString(Payload);
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public int CalculateSize() {
      int size = 0;
      if (FileChecksum.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(FileChecksum);
      }
      if (Payload.Length != 0) {
        size += 1 + pb::CodedOutputStream.ComputeStringSize(Payload);
      }
      return size;
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(FileTransfer other) {
      if (other == null) {
        return;
      }
      if (other.FileChecksum.Length != 0) {
        FileChecksum = other.FileChecksum;
      }
      if (other.Payload.Length != 0) {
        Payload = other.Payload;
      }
    }

    [global::System.Diagnostics.DebuggerNonUserCodeAttribute]
    public void MergeFrom(pb::CodedInputStream input) {
      uint tag;
      while ((tag = input.ReadTag()) != 0) {
        switch(tag) {
          default:
            input.SkipLastField();
            break;
          case 10: {
            FileChecksum = input.ReadString();
            break;
          }
          case 18: {
            Payload = input.ReadString();
            break;
          }
        }
      }
    }

  }

  #endregion

}

#endregion Designer generated code
