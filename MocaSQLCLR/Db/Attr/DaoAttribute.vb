
Imports System.Reflection
Imports Moca.Aop
Imports Moca.Attr
Imports Moca.Di
Imports Moca.Interceptor
Imports Moca.Util

Namespace Db.Attr

	''' <summary>
	''' DAO����
	''' </summary>
	''' <remarks>
	''' <see cref="IDao"/> �����������N���X���w�肷�鑮��
	''' </remarks>
	<AttributeUsage(AttributeTargets.Interface)> _
	Public Class DaoAttribute
		Inherits DbmsAttribute

		''' <summary>�w�肳�ꂽ���̉�����N���X�^�C�v</summary>
		Private _type As Type

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="typ">�N���X�^�C�v</param>
		''' <remarks>
		''' "context connection=true" ���g���ꍇ
		''' </remarks>
		Public Sub New(ByVal typ As Type)
			MyBase.New("context connection=true")
			_type = typ
		End Sub

		''' <summary>
		''' �R���X�g���N�^
		''' </summary>
		''' <param name="connectionString">ConnectionString</param>
		''' <param name="typ">�N���X�^�C�v</param>
		''' <remarks></remarks>
		Public Sub New(ByVal connectionString As String, ByVal typ As Type)
			MyBase.New(connectionString)
			_type = typ
		End Sub

#End Region
#Region " �v���p�e�B "

		''' <summary>
		''' �N���X�^�C�v�v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public ReadOnly Property ImplType() As Type
			Get
				Return _type
			End Get
		End Property

#End Region

		''' <summary>
		''' �R���|�[�l���g�쐬
		''' </summary>
		''' <param name="field">�t�B�[���h</param>
		''' <returns>DBMS�R���|�[�l���g</returns>
		''' <remarks></remarks>
		Public Function CreateComponent(ByVal field As FieldInfo) As MocaComponent
			' �^�`�F�b�N
			If Not ClassUtil.IsInterfaceImpl(ImplType, GetType(IDao)) Then
				Throw New ArgumentException(ImplType.FullName & " �́A" & GetType(IDao).FullName & " �����������N���X�ł͂���܂���B")
			End If

			' DBMS ����
			Dim targetDbms As Dbms
			targetDbms = DbmsManager.GetDbms(ConnectionString)

			Dim aspects As ArrayList
			Dim fields() As FieldInfo

			aspects = New ArrayList()

			' ����Ƀt�B�[���h�����
			fields = ClassUtil.GetFields(_type)
			For Each fi As FieldInfo In fields

				If ClassUtil.GetCustomAttribute(Of DaoAttribute)(fi.FieldType) Is Nothing Then
					If ClassUtil.GetCustomAttribute(Of ImplementationAttribute)(fi.FieldType) Is Nothing Then
						Continue For
					End If
				End If

				' Getter/Setter ���\�b�h�̃A�X�y�N�g�쐬�i�t�B�[���h�փA�N�Z�X���邽�߂ɕK�v�I�j
				Dim pointcut As IPointcut
				pointcut = New Pointcut(New String() {"Void FieldGetter(System.String, System.String, System.Object ByRef)"})
				aspects.Add(New Aspect(New FieldGetterInterceptor(), pointcut))
				pointcut = New Pointcut(New String() {"Void FieldSetter(System.String, System.String, System.Object)"})
				aspects.Add(New Aspect(New FieldSetterInterceptor(), pointcut))
			Next

			Dim component As MocaComponent4Db
			component = New MocaComponent4Db(_type, field.FieldType, targetDbms)
			component.Aspects = DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())
			Return component
		End Function

	End Class

End Namespace
