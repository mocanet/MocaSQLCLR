
Imports System.Reflection
Imports Moca.Util
Imports Moca.Attr
Imports Moca.Db
Imports Moca.Db.Attr

Namespace Di

	''' <summary>
	''' �ˑ����̒���
	''' </summary>
	''' <remarks>
	''' �C���^�t�F�[�X�̑����Ɏw�肳�ꂽ���ԃN���X���C���X�^���X�����t�B�[���h�֒�������B<br/>
	''' �����N���X�̎w��́A<see cref="ImplementationAttribute"/> �������g�p���Ďw�肵�܂��B<br/>
	''' �����Ŏw�肳�ꂽ�C���X�^���X�̃t�B�[���h�ɑ΂��āA���̑������w�肳�ꂽ�C���^�t�F�[�X�����݂����Ƃ��́A
	''' �����ŃC���X�^���X�����ăt�B�[���h�ɒ������܂��B<br/>
	''' ����āA�����̃t�B�[���h�̓C���X�^���X���i<c>New</c>�j����K�v�͂���܂���B<br/>
	''' ���C���X�^���X������Ȃ� <see cref="ImplementationAttribute"/> �����͎w�肵�Ȃ��ł��������B<br/>
	''' </remarks>
	Public Class MocaInjector
		Implements IDisposable

		''' <summary>�������</summary>
		Private _analyzer As AttributeAnalyzer

		Private _targets As ArrayList

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �f�t�H���g�R���X�g���N�^
		''' </summary>
		''' <remarks></remarks>
		Public Sub New()
			_analyzer = New AttributeAnalyzer

			' �f�t�H���g�̑�����͐ݒ�
			_analyzer.Add(AttributeAnalyzerTargets.Field, New ImplementationAttributeAnalyzer)
			_analyzer.Add(AttributeAnalyzerTargets.Field, New DaoAttributeAnalyzer)
			_analyzer.Add(AttributeAnalyzerTargets.Field, New TableAttributeAnalyzer)
			_analyzer.Add(AttributeAnalyzerTargets.Method, New AspectAttributeAnalyzer)
			_analyzer.Add(AttributeAnalyzerTargets.Method, New TransactionAttributeAnalyzer)

			_analyzer.AddIgnoreNamespace("System")
			_analyzer.AddIgnoreNamespace("Microsoft")
			_analyzer.AddIgnoreNamespace("log4net")
			_analyzer.AddIgnoreNamespace("Moca.Db.AbstractDao")

			_analyzer.FieldInject = AddressOf Me.fieldInject

			_targets = New ArrayList
		End Sub

#End Region
#Region " �v���p�e�B "

		''' <summary>
		''' ������̓v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected ReadOnly Property Analyzer() As AttributeAnalyzer
			Get
				Return _analyzer
			End Get
		End Property

#End Region
#Region " Dispose "

		Private disposedValue As Boolean = False		' �d������Ăяo�������o����ɂ�

		' IDisposable
		Protected Overridable Sub Dispose(ByVal disposing As Boolean)
			If Not Me.disposedValue Then
				If disposing Then
					' TODO: �����I�ɌĂяo���ꂽ�Ƃ��Ƀ}�l�[�W ���\�[�X��������܂�
				End If

				' TODO: ���L�̃A���}�l�[�W ���\�[�X��������܂�
				For Each target As Object In _targets
					Dim targetDispose As IDisposable
					DaoDispose(target)
					targetDispose = TryCast(target, IDisposable)
					If targetDispose IsNot Nothing Then
						targetDispose.Dispose()
					End If
				Next
			End If
			Me.disposedValue = True
		End Sub

#Region " IDisposable Support "
		' ���̃R�[�h�́A�j���\�ȃp�^�[���𐳂��������ł���悤�� Visual Basic �ɂ���Ēǉ�����܂����B
		Public Sub Dispose() Implements IDisposable.Dispose
			' ���̃R�[�h��ύX���Ȃ��ł��������B�N���[���A�b�v �R�[�h����� Dispose(ByVal disposing As Boolean) �ɋL�q���܂��B
			Dispose(True)
			GC.SuppressFinalize(Me)
		End Sub
#End Region

#End Region

		''' <summary>
		''' �ˑ����̒������ăC���X�^���X�𐶐�����
		''' </summary>
		''' <param name="target"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Overridable Function Create(ByVal target As Type) As Object
			Dim instance As Object
			instance = _analyzer.Create(target)
			_targets.Add(instance)
			Debug.WriteLine(String.Format("Inject -> {0}", target.GetType.FullName))
			Return instance
		End Function

		''' <summary>
		''' �ˑ����̒���
		''' </summary>
		''' <param name="target"></param>
		''' <remarks>
		''' �w�肳�ꂽ�C���X�^���X�̃t�B�[���h�� Interface �݂̂ɑΉ����Ă��܂��B
		''' </remarks>
		Public Overridable Sub Inject(ByVal target As Object)
			_analyzer.Analyze(target)
			_targets.Add(target)
			Debug.WriteLine(String.Format("Inject -> {0}", target.GetType.FullName))
		End Sub

		''' <summary>
		''' DAO �C���X�^���X�̊J��
		''' </summary>
		''' <param name="target"></param>
		''' <remarks>
		''' </remarks>
		Public Overridable Sub DaoDispose(ByVal target As Object)
			Dim fields() As FieldInfo

			If target Is Nothing Then
				Exit Sub
			End If

			' �t�B�[���h���`�F�b�N����
			fields = ClassUtil.GetFields(target)
			For Each field As FieldInfo In fields
				' Inject �����I�u�W�F�N�g�Ȃ�ċA��������
				If MocaContainerFactory.Container.GetComponent(field.GetType) IsNot Nothing Then
					DaoDispose(field.GetValue(target))
				End If

				' Dao ����������H
				Dim dbmsAttr As DbmsAttribute
				dbmsAttr = ClassUtil.GetCustomAttribute(Of DbmsAttribute)(field.FieldType)
				If dbmsAttr Is Nothing Then
					Continue For
				End If

				' Dispose ���s
				Dim dao As IDao
				dao = DirectCast(field.GetValue(target), IDao)
				If dao Is Nothing Then
					Continue For
				End If
				dao.Dispose()
				Debug.WriteLine(String.Format("DAO Dispose -> {0} ({1})", target.GetType.FullName, field.ToString))
			Next
		End Sub

		''' <summary>
		''' �t�B�[���h�փC���X�^���X�̒���
		''' </summary>
		''' <param name="target">�ΏۂƂȂ�I�u�W�F�N�g</param>
		''' <param name="field">�ΏۂƂȂ�t�B�[���h</param>
		''' <param name="component">�ΏۂƂȂ�R���|�[�l���g</param>
		''' <returns>���������C���X�^���X</returns>
		''' <remarks></remarks>
		Protected Function fieldInject(ByVal target As Object, ByVal field As FieldInfo, ByVal component As MocaComponent) As Object
			Dim instance As Object
			instance = component.Create(target)
			ClassUtil.Inject(target, field, New Object() {instance})
			Return instance
		End Function

	End Class

End Namespace
