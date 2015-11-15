
Imports Moca.Db

Namespace Di

	''' <summary>
	''' �R���e�i�Ɋi�[����f�[�^�x�[�X�������R���|�[�l���g
	''' </summary>
	''' <remarks></remarks>
	Public Class MocaComponent4Db
		Inherits MocaComponent

		''' <summary>����DBMS</summary>
		Private _dbms As Dbms

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="implType">���Ԃ̌^</param>
		''' <param name="fieldType">�t�B�[���h�̌^</param>
		''' <param name="targetDbms">�ΏۂƂȂ�DBMS</param>
		''' <remarks></remarks>
		Public Sub New(ByVal implType As Type, ByVal fieldType As Type, ByVal targetDbms As Dbms)
			MyBase.New(implType, fieldType)
			_dbms = targetDbms
		End Sub

#End Region
#Region " �v���p�e�B "

		''' <summary>
		''' DBMS
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property Dbms() As Dbms
			Get
				Return _dbms
			End Get
		End Property

#End Region

		''' <summary>
		''' �I�u�W�F�N�g���C���X�^���X�����ĕԂ��܂��B
		''' </summary>
		''' <returns></returns>
		''' <remarks>�I�[�o�[���C�h���\�b�h</remarks>
		Protected Overrides Function createObject(ByVal target As Object) As Object
			Dim val As Object
			val = MyBase.createObject(target)
			DirectCast(val, AbstractDao).TargetDbms = _dbms
			Return val
		End Function

		''' <summary>
		''' �I�u�W�F�N�g���v���L�V�Ƃ��ăC���X�^���X�����ĕԂ��܂��B
		''' </summary>
		''' <returns></returns>
		''' <remarks>�I�[�o�[���C�h���\�b�h</remarks>
		Protected Overrides Function createProxyObject(ByVal target As Object) As Object
			Dim val As Object
			val = MyBase.createProxyObject(target)

			Dim dao As AbstractDao
			dao = DirectCast(val, AbstractDao)
			dao.TargetDbms = _dbms

			Dim daoTarget As AbstractDao
			daoTarget = TryCast(target, AbstractDao)
			If daoTarget Is Nothing Then
				Return val
			End If

			Return val
		End Function

	End Class

End Namespace
