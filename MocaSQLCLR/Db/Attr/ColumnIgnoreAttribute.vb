
Namespace Db.Attr

	''' <summary>
	''' ���g�p����
	''' </summary>
	''' <remarks>
	''' ��Ƃ��Ďg�p���Ȃ��Ƃ��Ɏw�肷��B<br/>
	''' �w�肳��Ă��Ȃ��Ƃ��́A�u�g�p����v�ƂȂ�B
	''' </remarks>
	<AttributeUsage(AttributeTargets.Property)> _
	Public Class ColumnIgnoreAttribute
		Inherits Attribute

		''' <summary>�g�p�L��</summary>
		Private _ignore As Boolean

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="ignore">�g�p����Ƃ��� True�A�g�p���Ȃ��Ƃ��� False</param>
		''' <remarks></remarks>
		Public Sub New(ByVal ignore As Boolean)
			_ignore = ignore
		End Sub

#End Region

#Region " �v���p�e�B "

		''' <summary>
		''' �g�p�L���v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property Ignore() As Boolean
			Get
				Return _ignore
			End Get
		End Property

#End Region

	End Class

End Namespace
