Imports System.IO
''' <summary>
''' 用于表示NBS文件以供读取或修改
''' </summary>
Public Class NBSFile
    ''' <summary>
    ''' NBS文件的版本
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Version As Byte
    ''' <summary>
    ''' 获取NBS文件中包含原版乐器的数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property VanillaInstrumentCount As Byte
    ''' <summary>
    ''' 获取歌曲长度
    ''' 单位：tick
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property Songlength As Short
    ''' <summary>
    ''' 获取NBS文件中音轨的数量
    ''' </summary>
    ''' <returns></returns>
    Public ReadOnly Property LayerCount As Short
    ''' <summary>
    ''' 获取或设置歌曲名称
    ''' 由于NBS文件的特殊情况，遇到非ASCii字符此属性将会被清空
    ''' </summary>
    ''' <returns></returns>
    Public Property SongName As String
    ''' <summary>
    ''' 获取或设置作者名称
    ''' 由于NBS文件的特殊情况，遇到非ASCii字符此属性将会被清空
    ''' </summary>
    ''' <returns></returns>
    Public Property Author As String
    ''' <summary>
    ''' 获取或设置原始作者名称
    ''' 由于NBS文件的特殊情况，遇到非ASCii字符此属性将会被清空
    ''' </summary>
    ''' <returns></returns>
    Public Property OriginalAuthor As String
    ''' <summary>
    ''' 获取或设置歌曲描述
    ''' 由于NBS文件的特殊情况，遇到非ASCii字符此属性将会被清空
    ''' </summary>
    ''' <returns></returns>
    Public Property Description As String
    ''' <summary>
    ''' 获取或设置歌曲的运行速度
    ''' 请注意，此数值为速度乘以100的结果
    ''' 示例：1000代表10tick/s
    ''' </summary>
    ''' <returns></returns>
    Public Property Tempo As Short
    ''' <summary>
    ''' 表示歌曲是否会自动保存（已弃用）
    ''' </summary>
    ''' <returns></returns>
    Public Property AutoSaving As Byte
    ''' <summary>
    ''' 表示歌曲每次自动保存的长度（已弃用）
    ''' </summary>
    ''' <returns></returns>
    Public Property AutoSavingDuration As Byte
    ''' <summary>
    ''' 获取或设置歌曲的拍号
    ''' 范围为2到8
    ''' 示例：3表示3/4拍，4表示4/4拍
    ''' </summary>
    ''' <returns></returns>
    Public Property TimeSignature As Byte
    ''' <summary>
    ''' 获取或设置此NBS文件的制作时长
    ''' 单位：分钟
    ''' </summary>
    ''' <returns></returns>
    Public Property MinuteSpent As Integer
    ''' <summary>
    ''' 获取或设置制作此NBS文件时左键的点击次数
    ''' </summary>
    ''' <returns></returns>
    Public Property LeftClickTimes As Integer
    ''' <summary>
    ''' 获取或设置制作此NBS文件时右键的点击次数
    ''' </summary>
    ''' <returns></returns>
    Public Property RightClickTimes As Integer
    ''' <summary>
    ''' 获取或设置制作此NBS文件时添加的音符数量
    ''' </summary>
    ''' <returns></returns>
    Public Property NoteBlocksAdded As Integer
    ''' <summary>
    ''' 获取或设置制作此NBS文件时删除的音符数量
    ''' </summary>
    ''' <returns></returns>
    Public Property NoteBlocksRemoved As Integer
    ''' <summary>
    ''' 获取或设置此NBS文件是从什么MIDI文件导入的（如果有）
    ''' </summary>
    ''' <returns></returns>
    Public Property MIDIName As String
    ''' <summary>
    ''' 获取或设置制作此NBS文件是否开启了循环模式
    ''' </summary>
    ''' <returns></returns>
    Public Property IsLooping As Boolean
    ''' <summary>
    ''' 获取或设置最大循环次数
    ''' </summary>
    ''' <returns></returns>
    Public Property LoopTimes As Byte
    ''' <summary>
    ''' 获取或设置NBS文件的循环起始点
    ''' </summary>
    ''' <returns></returns>
    Public Property LoopStartTick As Short
    ''' <summary>
    ''' 用于表示NBS文件中的音轨
    ''' </summary>
    Public Layers As List(Of Layer)
    ''' <summary>
    ''' 创建空白的NBSFile实例以供操作
    ''' </summary>
    Public Sub New()
        Version = 0

    End Sub
    ''' <summary>
    ''' 通过现有的NBS文件创建新的NBSFile实例
    ''' </summary>
    ''' <param name="Location">NBS文件的路径</param>
    Public Sub New(Location As String)
        Dim fs As New FileStream(Location, FileMode.Open, FileAccess.Read)
        Dim reader As New BinaryReader(fs)
        If reader.ReadInt16 <> 0 Then
            Throw New Exception("不支持旧版本NBS文件")
            Exit Sub
        End If
        Dim stringlength As Integer
        Version = reader.ReadByte
        VanillaInstrumentCount = reader.ReadByte
        Songlength = reader.ReadInt16
        LayerCount = reader.ReadInt16
        stringlength = reader.ReadInt32
        SongName += reader.ReadChars(stringlength)
        stringlength = reader.ReadInt32
        Author = reader.ReadChars(stringlength)
        stringlength = reader.ReadInt32
        OriginalAuthor = reader.ReadChars(stringlength)
        stringlength = reader.ReadInt32
        Description = reader.ReadChars(stringlength)
        Tempo = reader.ReadInt16
        AutoSaving = reader.ReadByte
        AutoSavingDuration = reader.ReadByte
        TimeSignature = reader.ReadByte
        MinuteSpent = reader.ReadInt32
        LeftClickTimes = reader.ReadInt32
        RightClickTimes = reader.ReadInt32
        NoteBlocksAdded = reader.ReadInt32
        NoteBlocksRemoved = reader.ReadInt32
        stringlength = reader.ReadInt32
        MIDIName = reader.ReadChars(stringlength)
        IsLooping = reader.ReadBoolean
        LoopTimes = reader.ReadByte
        LoopStartTick = reader.ReadInt16
        Dim CurrentLayer As Integer
        Dim JumpTick As Short
        Dim JumpLayer As Short
        Dim NoteInstrument As Byte
        Dim NoteKey As Byte
        Dim NoteVelocity As Byte
        Dim NotePan As Byte
        Dim NotePitch As Short
        Dim CurrentTick As Short = -1
        Layers = New List(Of Layer)
        For i = 1 To LayerCount
            Layers.Add(New Layer)
        Next
        Do
            JumpTick = reader.ReadInt16
            If JumpTick = 0 Then
                Exit Do
            End If
            CurrentTick += JumpTick
            CurrentLayer = -1
            Do
                JumpLayer = reader.ReadInt16
                If JumpLayer = 0 Then
                    Exit Do
                End If
                CurrentLayer += JumpLayer
                NoteInstrument = reader.ReadByte
                NoteKey = reader.ReadByte
                NoteVelocity = reader.ReadByte
                NotePan = reader.ReadByte
                NotePitch = reader.ReadInt16
                Layers(CurrentLayer).Notes.Add(New Note(CurrentTick, NoteInstrument, NoteKey, NoteVelocity, NotePan, NotePitch))
            Loop
        Loop
        For i = 0 To LayerCount - 1
            stringlength = reader.ReadInt32
            Layers(i).LayerName = reader.ReadChars(stringlength)
            Layers(i).Locked = reader.ReadByte
            Layers(i).Volume = reader.ReadByte
            Layers(i).Stereo = reader.ReadByte
            Layers(i).Arrange()
        Next
        ClearNotASCII()
    End Sub
    Private Sub ClearNotASCII()
        For Each c In SongName
            If AscW(c) > 128 Then
                SongName = ""
                Exit For
            End If
        Next
        For Each c In Author
            If AscW(c) >= 128 Then
                Author = ""
                Exit For
            End If
        Next
        For Each c In OriginalAuthor
            If AscW(c) >= 128 Then
                OriginalAuthor = ""
                Exit For
            End If
        Next
        For Each c In Description
            If AscW(c) >= 128 Then
                Description = ""
                Exit For
            End If
        Next
        For Each l In Layers
            For Each c In l.LayerName
                If AscW(c) >= 128 Then
                    l.LayerName = ""
                    Exit For
                End If
            Next
        Next
    End Sub
    ''' <summary>
    ''' 将NBSFile实例储存为NBS文件
    ''' </summary>
    ''' <param name="Path">需要保存的目标文件</param>
    Public Sub Save(Path As String)
        ClearNotASCII()
        Dim fs As New FileStream(Path, FileMode.Create)
        Dim writer As New BinaryWriter(fs)
        Dim oldlength As Short = 0
        Dim jumplayer As Short = 1
        Dim notes As New List(Of Note)
        Dim lasttick As Short = -1
        Dim currentlayer As Short = -1
        With writer
            .Write(oldlength)
            .Write(Version)
            .Write(VanillaInstrumentCount)
            .Write(Songlength)
            .Write(LayerCount)
            .Write(Len(SongName))
            .Write(SongName, 0, Len(SongName))
            .Write(Len(Author))
            .Write(Author, 0, Len(Author))
            .Write(Len(OriginalAuthor))
            .Write(OriginalAuthor, 0, Len(OriginalAuthor))
            .Write(Len(Description))
            .Write(Description, 0, Len(Description))
            .Write(Tempo)
            .Write(AutoSaving)
            .Write(AutoSavingDuration)
            .Write(TimeSignature)
            .Write(MinuteSpent)
            .Write(LeftClickTimes)
            .Write(RightClickTimes)
            .Write(NoteBlocksAdded)
            .Write(NoteBlocksRemoved)
            .Write(Len(MIDIName))
            .Write(MIDIName, 0, Len(MIDIName))
            .Write(IsLooping)
            .Write(LoopTimes)
            .Write(LoopStartTick)
        End With
        For Each l In Layers
            currentlayer += 1
            For Each n In l.Notes
                n.LayerID = currentlayer
                notes.Add(n)
            Next
        Next
        notes.Sort(Function(a, b) a.Tick.CompareTo(b.Tick))
        For Each n In notes
            If n.Tick > lasttick Then
                If lasttick > -1 Then
                    writer.Write(oldlength)
                End If
                writer.Write(n.Tick - lasttick)
                lasttick = n.Tick
                currentlayer = -1
            End If
            writer.Write(n.LayerID - currentlayer)
            currentlayer = n.LayerID
            writer.Write(n.Instrument)
            writer.Write(n.NoteKey)
            writer.Write(n.NoteVelocity)
            writer.Write(n.NotePan)
            writer.Write(n.NotePitch)
        Next
        writer.Write(0)
        For Each l In Layers
            writer.Write(Len(l.LayerName))
            writer.Write(l.LayerName, 0, Len(l.LayerName))
            writer.Write(l.Locked)
            writer.Write(l.Volume)
            writer.Write(l.Stereo)
        Next
        fs.WriteByte(0)
        writer.Flush()
        writer.Close()
    End Sub
End Class
''' <summary>
''' 用于表示NBS的音符
''' </summary>
Public Class Note
    ''' <summary>
    ''' 获取或设置音符出现的时间
    ''' 单位：tick
    ''' </summary>
    ''' <returns></returns>
    Public Property Tick As Short
    ''' <summary>
    ''' 获取或设置音符的乐器编号
    ''' </summary>
    ''' <returns></returns>
    Public Property Instrument As Byte
    ''' <summary>
    ''' 获取或设置音符的音高
    ''' 范围为0-87
    ''' 其中33-57为Minecraft指定的两个八度
    ''' </summary>
    ''' <returns></returns>
    Public Property NoteKey As Byte
    ''' <summary>
    ''' 获取或设置音符的力度
    ''' 范围为0-100
    ''' </summary>
    ''' <returns></returns>
    Public Property NoteVelocity As Byte
    ''' <summary>
    ''' 获取或设置音符的发声位置
    ''' 范围为0-200
    ''' 0为右侧2个方块，100为正中央，200为左侧两个方块
    ''' </summary>
    ''' <returns></returns>
    Public Property NotePan As Byte
    ''' <summary>
    ''' 获取或设置音符的细微音高
    ''' 范围为-1200到1200
    ''' 每2个半音之间相差100
    ''' </summary>
    ''' <returns></returns>
    Public Property NotePitch As Short
    Protected Friend Property LayerID As Short
    ''' <summary>
    ''' 创建新的Note实例
    ''' </summary>
    ''' <param name="Tick">音符出现的时间</param>
    ''' <param name="Instrument">音符的乐器编号</param>
    ''' <param name="NoteKey">音符的音高</param>
    ''' <param name="Notevelocity">音符的力度</param>
    ''' <param name="NotePan">音符的发声位置</param>
    ''' <param name="NotePitch">音符的细微音高</param>
    Public Sub New(Tick As Short, Instrument As Instruments, NoteKey As Byte, Notevelocity As Byte, NotePan As Byte, NotePitch As Short)
        Me.Tick = Tick
        Me.Instrument = Instrument
        Me.NoteKey = NoteKey
        Me.NoteVelocity = Notevelocity
        Me.NotePan = NotePan
        Me.NotePitch = NotePitch
    End Sub
End Class
''' <summary>
''' 用于表示NBS的音轨
''' </summary>
Public Class Layer
    ''' <summary>
    ''' 表示此音轨中包含的音符
    ''' </summary>
    Public Notes As List(Of Note)
    ''' <summary>
    ''' 获取或设置音轨的名称
    ''' 由于NBS文件的特殊情况，遇到非ASCii字符此属性将会被清空
    ''' </summary>
    ''' <returns></returns>
    Public Property LayerName As String
    ''' <summary>
    ''' 获取或设置音轨是否处于锁定状态
    ''' </summary>
    ''' <returns></returns>
    Public Property Locked As Boolean
    ''' <summary>
    ''' 获取或设置音轨的音量
    ''' 范围为0-100
    ''' </summary>
    ''' <returns></returns>
    Public Property Volume As Byte
    ''' <summary>
    ''' 获取或设置音轨的声道平衡
    ''' 范围为0-200
    ''' 0为右侧2个方块，100为正中央，200为左侧两个方块
    ''' </summary>
    ''' <returns></returns>
    Public Property Stereo As Byte
    ''' <summary>
    ''' 创建新的Layer实例
    ''' </summary>
    Public Sub New()
        Notes = New List(Of Note)
        Locked = False
        Volume = 100
        Stereo = 100
    End Sub
    ''' <summary>
    ''' 整理音轨
    ''' </summary>
    Public Sub Arrange()

        Notes.Sort(Function(a, b) a.Tick.CompareTo(b.Tick))
    End Sub
End Class
''' <summary>
''' 表示原版音符盒音色列表
''' 命名使用Minecraft文件名
''' </summary>
Public Enum Instruments As Byte
    ''' <summary>
    ''' 竖琴，泥土音色
    ''' </summary>
    harp2 = 0
    ''' <summary>
    ''' 贝司，木板音色
    ''' </summary>
    bassattack = 1
    ''' <summary>
    ''' 大鼓，石头音色
    ''' </summary>
    bd = 2
    ''' <summary>
    ''' 小鼓，沙子音色
    ''' </summary>
    snare = 3
    ''' <summary>
    ''' 鼓沿，玻璃音色
    ''' </summary>
    hat = 4
    ''' <summary>
    ''' 吉他，羊毛音色
    ''' </summary>
    guitar = 5
    ''' <summary>
    ''' 竖笛，粘土音色
    ''' </summary>
    flute = 6
    ''' <summary>
    ''' 铃铛，金块音色
    ''' </summary>
    bell = 7
    ''' <summary>
    ''' 管钟，浮冰音色
    ''' </summary>
    icechime = 8
    ''' <summary>
    ''' 木琴，骨块音色
    ''' </summary>
    xylobone = 9
    ''' <summary>
    ''' 钢片琴，铁块音色
    ''' </summary>
    iron_xylophone = 10
    ''' <summary>
    ''' 牛铃，灵魂沙音色
    ''' </summary>
    cow_bell = 11
    ''' <summary>
    ''' 大管，南瓜音色
    ''' </summary>
    didgeridoo = 12
    ''' <summary>
    ''' 方波，绿宝石块音色
    ''' </summary>
    bit = 13
    ''' <summary>
    ''' 班卓，干草块音色
    ''' </summary>
    banjo = 14
    ''' <summary>
    ''' 电钢琴，萤石块音色
    ''' </summary>
    pling = 15
End Enum