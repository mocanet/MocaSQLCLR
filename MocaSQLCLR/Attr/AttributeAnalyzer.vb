
Imports System.Reflection
Imports Moca.Aop
Imports Moca.Di
Imports Moca.Util
Imports Moca.Interceptor

Namespace Attr

	''' <summary>
	''' �t�B�[���h�C���W�F�N�g�f���Q�[�g
	''' </summary>
	''' <param name="target">�C���W�F�N�g�ΏۂƂȂ�C���X�^���X</param>
	''' <param name="field">�ΏۂƂȂ�t�B�[���h��`</param>
	''' <param name="component">�C���W�F�N�g����R���|�[�l���g</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Delegate Function MocaFieldInject(ByVal target As Object, ByVal field As FieldInfo, ByVal component As MocaComponent) As Object

	''' <summary>
	''' ����������
	''' </summary>
	''' <param name="parent"></param>
	''' <param name="obj"></param>
	''' <remarks></remarks>
	Friend Delegate Sub MocaEventDelegateInject(ByVal parent As Object, ByVal obj As Object)

#Region " �񋓌^ "

	''' <summary>
	''' ������͂���^�[�Q�b�g�񋓌^
	''' </summary>
	''' <remarks></remarks>
	Public Enum AttributeAnalyzerTargets
		''' <summary>�N���X</summary>
		[Class] = AttributeTargets.Class
		''' <summary>�t�B�[���h</summary>
		Field = AttributeTargets.Field
		''' <summary>�C���^�t�F�[�X</summary>
		[Interface] = AttributeTargets.Interface
		''' <summary>���\�b�h</summary>
		Method = AttributeTargets.Method
		''' <summary>�v���p�e�B</summary>
		[Property] = AttributeTargets.Property
	End Enum

#End Region

	''' <summary>
	''' �������
	''' </summary>
	''' <remarks></remarks>
	Public Class AttributeAnalyzer

		''' <summary>�e���͂���</summary>
		Private _analyzers As Dictionary(Of AttributeAnalyzerTargets, IList(Of IAttributeAnalyzer))
		''' <summary>��͂����O����Namespace</summary>
		Private _ignoreNamespace As IList(Of String)

		''' <summary>�t�B�[���h�C���W�F�N�g�f���Q�[�g</summary>
		Private _injectMethod As MocaFieldInject

		''' <summary>������</summary>
		Private _injectEventDelegate As MocaEventDelegateInject

#Region " �R���X�g���N�^ "

		''' <summary>
		''' �f�t�H���g�R���X�g���N�^
		''' </summary>
		''' <remarks></remarks>
		Public Sub New()
			_analyzers = New Dictionary(Of AttributeAnalyzerTargets, IList(Of IAttributeAnalyzer))
			_ignoreNamespace = New List(Of String)
			_injectEventDelegate = Nothing
		End Sub

#End Region

#Region " ��� "

#Region " ���� "

		''' <summary>
		''' �N���X����͂��ăC���X�^���X�𐶐�����
		''' </summary>
		''' <param name="typ"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Function Create(ByVal typ As Type) As Object
			Dim component As MocaComponent
			Dim aspects As ArrayList

			component = MocaContainerFactory.Container().GetComponent(typ)
			If component IsNot Nothing Then
				Return getInstance(component)
			End If

			component = analyzeClass(typ)
			If component Is Nothing Then
				component = New Moca.Di.MocaComponent(typ, typ)
			End If

			' ���������R���|�[�l���g�� Di ��
			MocaContainerFactory.Container().SetComponent(component)

			aspects = New ArrayList()
			aspects.AddRange(analyzeInterfaces(typ))				' �C���^�t�F�[�X���

			' Getter/Setter ���\�b�h�̃A�X�y�N�g�쐬�i�t�B�[���h�փA�N�Z�X���邽�߂ɕK�v�I�j
			aspects.AddRange(_createFieldGetterSetterAspect())

			component.Aspects = DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())

			Dim instance As Object
			instance = getInstance(component)

			Analyze(instance)

			Return instance
		End Function

		''' <summary>
		''' �t�B�[���h���
		''' </summary>
		''' <param name="target">�ΏۂƂȂ�I�u�W�F�N�g</param>
		''' <returns>�쐬�����R���|�[�l���g</returns>
		''' <remarks></remarks>
		Protected Function analyzeClass(ByVal target As Type) As MocaComponent
			Dim component As MocaComponent

			component = Nothing

			If _analyzers.ContainsKey(AttributeAnalyzerTargets.Class) Then
				For Each analyzer As IAttributeAnalyzer In _analyzers(AttributeAnalyzerTargets.Class)
					component = analyzer.Analyze(target)
					If component IsNot Nothing Then
						Exit For
					End If
				Next
			End If

			Return component
		End Function

#End Region

		''' <summary>
		''' ���
		''' </summary>
		''' <param name="target">�ΏۂƂȂ�I�u�W�F�N�g</param>
		''' <remarks>
		''' ��͂��J�n����O�ɉ�͂��������̃A�i���C�U�[��ǉ����Ă��������B
		''' </remarks>
		Public Sub Analyze(ByVal target As Object)
			For Each field As FieldInfo In ClassUtil.GetFields(target)
				Dim component As MocaComponent

				If _isIgnore(field.DeclaringType.ToString) Then
					Continue For
				End If

				' ���ɑ��݂��邩�`�F�b�N�i�t�B�[���h�̌^�Łj
				component = MocaContainerFactory.Container().GetComponent(field.FieldType)
				If component IsNot Nothing Then
					Analyze(Me.FieldInject(target, field, component))
					Continue For
				End If

				' �t�B�[���h�̉��
				Analyze(target, field)
			Next

			' �C�x���g�̃f���Q�[�g����͂��邩�ǂ���
			If Me.EventDelegateInject Is Nothing Then
				Return
			End If

			'_mylog.DebugFormat("AnalyzeEventDelegate Type={0},Name={1}", target.GetType.Name, "Nothing")
			'Me.EventDelegateInject.Invoke(target, target)

			For Each prop As PropertyInfo In ClassUtil.GetProperties(target)
				analyzeEventDelegate(target, prop)
			Next
		End Sub

#Region " �t�B�[���h��� "

		''' <summary>
		''' �t�B�[���h���
		''' </summary>
		''' <param name="target">�ΏۂƂȂ�I�u�W�F�N�g</param>
		''' <param name="field">�ΏۂƂȂ�t�B�[���h</param>
		''' <remarks></remarks>
		Protected Overridable Sub analyze(ByVal target As Object, ByVal field As FieldInfo)
			Dim component As MocaComponent
			Dim aspects As ArrayList

			aspects = New ArrayList()

			' �t�B�[���h��́i���R���|�[�l���g�����j
			component = analyzeField(target, field)
			If component Is Nothing Then
				Exit Sub
			End If

			' ���ɑ��݂��邩�`�F�b�N�i���ԂŁj
			If component.ImplType Is Nothing Then
				If MocaContainerFactory.Container().GetComponent(component.Key) IsNot Nothing Then
					component = MocaContainerFactory.Container().GetComponent(component.Key)
					Analyze(Me.FieldInject(target, field, component))
					Exit Sub
				End If
			Else
				If MocaContainerFactory.Container().GetComponent(component.ImplType) IsNot Nothing Then
					component = MocaContainerFactory.Container().GetComponent(component.ImplType)
					Analyze(Me.FieldInject(target, field, component))
					Exit Sub
				End If
			End If

			' ���������R���|�[�l���g�� Di ��
			MocaContainerFactory.Container().SetComponent(component)

			' �����o���
			aspects.AddRange(analyzeInterfaces(field.FieldType))

			' �t�B�[���h�̌^�Ǝ��Ԃ̌^���قȂ邩�H
			' �قȂ�Ƃ��́A���Ԃł������o���
			If Not field.FieldType.Equals(component.ImplType) Then
				aspects.AddRange(analyzeProperty(component.ImplType))	' �v���p�e�B���
				aspects.AddRange(analyzeMethod(component.ImplType))		' ���\�b�h���
				'aspects.AddRange(analyzeEvent(component.ImplType))      ' �C�x���g���

				' Getter/Setter ���\�b�h�̃A�X�y�N�g�쐬�i�t�B�[���h�փA�N�Z�X���邽�߂ɕK�v�I�j
				aspects.AddRange(_createFieldGetterSetterAspect())
			End If

			' �R���|�[�l���g�փA�X�y�N�g�ݒ�
			If component.Aspects IsNot Nothing Then
				aspects.InsertRange(0, component.Aspects)
			End If
			component.Aspects = DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())

			' �t�B�[���h�փC���X�^���X�𒍓����A
			' �C���X�^���X�������I�u�W�F�N�g�ŉ�͂��ċA
			Analyze(Me.FieldInject(target, field, component))
		End Sub

		Protected Overridable Sub analyze(ByVal target As Type, ByVal field As FieldInfo)
			Dim component As MocaComponent
			Dim aspects As ArrayList

			aspects = New ArrayList()

			' �t�B�[���h��́i���R���|�[�l���g�����j
			component = analyzeField(target, field)
			If component Is Nothing Then
				Exit Sub
			End If

			' ���ɑ��݂��邩�`�F�b�N�i���ԂŁj
			If component.ImplType Is Nothing Then
				If MocaContainerFactory.Container().GetComponent(component.Key) IsNot Nothing Then
					component = MocaContainerFactory.Container().GetComponent(component.Key)
					Analyze(Me.FieldInject(target, field, component))
					Exit Sub
				End If
			Else
				If MocaContainerFactory.Container().GetComponent(component.ImplType) IsNot Nothing Then
					component = MocaContainerFactory.Container().GetComponent(component.ImplType)
					Analyze(Me.FieldInject(target, field, component))
					Exit Sub
				End If
			End If

			' ���������R���|�[�l���g�� Di ��
			MocaContainerFactory.Container().SetComponent(component)

			' �����o���
			aspects.AddRange(analyzeInterfaces(field.FieldType))

			' �t�B�[���h�̌^�Ǝ��Ԃ̌^���قȂ邩�H
			' �قȂ�Ƃ��́A���Ԃł������o���
			If Not field.FieldType.Equals(component.ImplType) Then
				aspects.AddRange(analyzeProperty(component.ImplType))	' �v���p�e�B���
				aspects.AddRange(analyzeMethod(component.ImplType))		' ���\�b�h���
				'aspects.AddRange(analyzeEvent(component.ImplType))      ' �C�x���g���

				' Getter/Setter ���\�b�h�̃A�X�y�N�g�쐬�i�t�B�[���h�փA�N�Z�X���邽�߂ɕK�v�I�j
				aspects.AddRange(_createFieldGetterSetterAspect())
			End If

			' �R���|�[�l���g�փA�X�y�N�g�ݒ�
			If component.Aspects IsNot Nothing Then
				aspects.InsertRange(0, component.Aspects)
			End If
			component.Aspects = DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())

			' �t�B�[���h�փC���X�^���X�𒍓����A
			' �C���X�^���X�������I�u�W�F�N�g�ŉ�͂��ċA
			Analyze(Me.FieldInject(target, field, component))
		End Sub

		Private Function _createFieldGetterSetterAspect() As IAspect()
			Dim aspects As ArrayList
			Dim pointcut As IPointcut

			aspects = New ArrayList()

			pointcut = New Pointcut(New String() {"Void FieldGetter(System.String, System.String, System.Object ByRef)"})
			aspects.Add(New Aspect(New FieldGetterInterceptor(), pointcut))
			pointcut = New Pointcut(New String() {"Void FieldSetter(System.String, System.String, System.Object)"})
			aspects.Add(New Aspect(New FieldSetterInterceptor(), pointcut))

			Return DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())
		End Function

		''' <summary>
		''' �C���^�t�F�[�X�̌p������H��
		''' </summary>
		''' <param name="targetTyp">�Ώۂ�Type</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Overridable Function analyzeInterfaces(ByVal targetTyp As Type) As IAspect()
			Dim aspects As ArrayList

			aspects = New ArrayList()

			aspects.AddRange(analyzeProperty(targetTyp))	' �v���p�e�B���
			aspects.AddRange(analyzeMethod(targetTyp))		' ���\�b�h���
			'aspects.AddRange(analyzeEvent(targetTyp))	   ' �C�x���g���

			If targetTyp.GetInterfaces().Length = 0 Then
				Return DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())
			End If
			For Each typ As Type In targetTyp.GetInterfaces
				aspects.AddRange(analyzeInterfaces(typ))
			Next

			Return DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())
		End Function

		''' <summary>
		''' �t�B�[���h���
		''' </summary>
		''' <param name="target">�ΏۂƂȂ�I�u�W�F�N�g</param>
		''' <param name="field">�t�B�[���h</param>
		''' <returns>�쐬�����R���|�[�l���g</returns>
		''' <remarks></remarks>
		Protected Overridable Function analyzeField(ByVal target As Object, ByVal field As FieldInfo) As MocaComponent
			Dim component As MocaComponent

			component = Nothing

			If _isIgnore(field.DeclaringType.FullName) Then
				Return component
			End If
			If Not String.IsNullOrEmpty(field.FieldType.Namespace) Then
				If field.FieldType.Namespace.StartsWith("log4net") Then
					Return component
				End If
			End If

			If _analyzers.ContainsKey(AttributeAnalyzerTargets.Field) Then
				For Each analyzer As IAttributeAnalyzer In _analyzers(AttributeAnalyzerTargets.Field)
					component = analyzer.Analyze(target, field)
					If component IsNot Nothing Then
						Exit For
					End If
				Next
			End If

			Return component
		End Function

#End Region
#Region " �v���p�e�B��� "

		''' <summary>
		''' �v���p�e�B���
		''' </summary>
		''' <param name="targetType">�ΏۂƂȂ�^</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Overridable Function analyzeProperty(ByVal targetType As Type) As IAspect()
			Dim aspects As ArrayList

			aspects = New ArrayList()

			If targetType Is Nothing Then
				Return DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())
			End If

			For Each prop As PropertyInfo In ClassUtil.GetProperties(targetType)
				Dim rc() As IAspect
				rc = analyzeProperty(targetType, prop)
				If rc Is Nothing OrElse rc.Length = 0 Then
					Continue For
				End If
				aspects.AddRange(rc)
				aspects.AddRange(analyzeMethod(prop.PropertyType))		' ���\�b�h���
			Next

			Return DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())
		End Function

		''' <summary>
		''' �v���p�e�B���
		''' </summary>
		''' <param name="typ">�ΏۂƂȂ�^</param>
		''' <param name="prop">�v���p�e�B</param>
		''' <returns>�A�X�y�N�g�z��</returns>
		''' <remarks></remarks>
		Protected Overridable Function analyzeProperty(ByVal typ As Type, ByVal prop As PropertyInfo) As IAspect()
			Dim results As ArrayList
			Dim aspects() As IAspect

			results = New ArrayList()

			If _isIgnore(prop.DeclaringType.FullName) Then
				Return DirectCast(results.ToArray(GetType(IAspect)), IAspect())
			End If

			If _analyzers.ContainsKey(AttributeAnalyzerTargets.Property) Then
				For Each analyzer As IAttributeAnalyzer In _analyzers(AttributeAnalyzerTargets.Property)
					aspects = analyzer.Analyze(typ, prop)
					If aspects Is Nothing Then
						Continue For
					End If
					results.AddRange(aspects)
				Next
			End If

			Return DirectCast(results.ToArray(GetType(IAspect)), IAspect())
		End Function

#End Region
#Region " ���\�b�h��� "

		''' <summary>
		''' ���\�b�h���
		''' </summary>
		''' <param name="targetType">�ΏۂƂȂ�^</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Overridable Function analyzeMethod(ByVal targetType As Type) As IAspect()
			Dim aspects As ArrayList

			aspects = New ArrayList()

			If targetType Is Nothing Then
				Return DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())
			End If

			For Each method As MethodInfo In ClassUtil.GetMethods(targetType)
				Dim rc() As IAspect
				rc = analyzeMethod(targetType, method)
				If rc Is Nothing OrElse rc.Length = 0 Then
					Continue For
				End If
				aspects.AddRange(rc)
			Next

			Return DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())
		End Function

		''' <summary>
		''' ���\�b�h���
		''' </summary>
		''' <param name="typ">�ΏۂƂȂ�^</param>
		''' <param name="method">���\�b�h</param>
		''' <returns>�A�X�y�N�g�z��</returns>
		''' <remarks></remarks>
		Protected Overridable Function analyzeMethod(ByVal typ As Type, ByVal method As MethodInfo) As IAspect()
			Dim results As ArrayList
			Dim aspects() As IAspect

			results = New ArrayList()

			If _isIgnore(method.DeclaringType.ToString) Then
				Return DirectCast(results.ToArray(GetType(IAspect)), IAspect())
			End If

			If _analyzers.ContainsKey(AttributeAnalyzerTargets.Method) Then
				For Each analyzer As IAttributeAnalyzer In _analyzers(AttributeAnalyzerTargets.Method)
					aspects = analyzer.Analyze(typ, method)
					If aspects Is Nothing Then
						Continue For
					End If
					results.AddRange(aspects)
				Next
			End If

			Return DirectCast(results.ToArray(GetType(IAspect)), IAspect())
		End Function

#End Region
#Region " �C�x���g��� "

		''' <summary>
		''' �C�x���g�f���Q�[�g���
		''' </summary>
		''' <param name="parent"></param>
		''' <param name="prop"></param>
		''' <remarks></remarks>
		Protected Overridable Sub analyzeEventDelegate(ByVal parent As Object, ByVal prop As PropertyInfo)
			Dim obj As Object

			obj = prop.GetValue(parent, New Object() {})
			If obj Is Nothing Then
				Return
			End If

			Me.EventDelegateInject.Invoke(parent, obj)
		End Sub

		''' <summary>
		''' �C�x���g���
		''' </summary>
		''' <param name="targetType">�ΏۂƂȂ�^</param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Overridable Function analyzeEvent(ByVal targetType As Type) As IAspect()
			Dim aspects As ArrayList

			aspects = New ArrayList()

			If targetType Is Nothing Then
				Return DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())
			End If

			For Each method As EventInfo In ClassUtil.GetEvents(targetType)
				Dim rc() As IAspect
				rc = analyzeEvent(targetType, method)
				If rc Is Nothing OrElse rc.Length = 0 Then
					Continue For
				End If
				aspects.AddRange(rc)
			Next

			Return DirectCast(aspects.ToArray(GetType(IAspect)), IAspect())
		End Function

		''' <summary>
		''' �C�x���g���
		''' </summary>
		''' <param name="typ">�ΏۂƂȂ�^</param>
		''' <param name="method">�C�x���g</param>
		''' <returns>�A�X�y�N�g�z��</returns>
		''' <remarks></remarks>
		Protected Overridable Function analyzeEvent(ByVal typ As Type, ByVal method As EventInfo) As IAspect()
			Dim results As ArrayList
			Dim aspects() As IAspect

			results = New ArrayList()

			If _isIgnore(method.DeclaringType.FullName) Then
				Return DirectCast(results.ToArray(GetType(IAspect)), IAspect())
			End If

			If _analyzers.ContainsKey(AttributeAnalyzerTargets.Method) Then
				For Each analyzer As IAttributeAnalyzer In _analyzers(AttributeAnalyzerTargets.Method)
					aspects = analyzer.Analyze(typ, method)
					If aspects Is Nothing Then
						Continue For
					End If
					results.AddRange(aspects)
				Next
			End If

			Return DirectCast(results.ToArray(GetType(IAspect)), IAspect())
		End Function

#End Region

		' ''' <summary>
		' ''' �t�B�[���h�փC���X�^���X�̒���
		' ''' </summary>
		' ''' <param name="target">�ΏۂƂȂ�I�u�W�F�N�g</param>
		' ''' <param name="field">�ΏۂƂȂ�t�B�[���h</param>
		' ''' <param name="component">�ΏۂƂȂ�R���|�[�l���g</param>
		' ''' <returns>���������C���X�^���X</returns>
		' ''' <remarks></remarks>
		'Protected Overridable Function inject(ByVal target As Object, ByVal field As FieldInfo, ByVal component As MocaComponent) As Object
		'	Dim instance As Object
		'	instance = getInstance(target, component)
		'	ClassUtil.Inject(target, field, New Object() {instance})
		'	Return instance
		'End Function

		''' <summary>
		''' �R���|�[�l���g������ԉ�
		''' </summary>
		''' <param name="component"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Overridable Function getInstance(ByVal component As MocaComponent) As Object
			Return component.Create()
		End Function

		''' <summary>
		''' �R���|�[�l���g������ԉ�
		''' </summary>
		''' <param name="component"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Protected Overridable Function getInstance(ByVal target As Object, ByVal component As MocaComponent) As Object
			Return component.Create(target)
		End Function

		''' <summary>
		''' ��͏��O�`�F�b�N
		''' </summary>
		''' <param name="val"></param>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Function _isIgnore(ByVal val As String) As Boolean
			For Each chkVal As String In _ignoreNamespace
				If val.StartsWith(chkVal) Then
					Return True
				End If
			Next

			Return False
		End Function

#End Region

		''' <summary>
		''' �t�B�[���h�C���W�F�N�V�����f���Q�[�g�v���p�e�B
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Public Property FieldInject As MocaFieldInject
			Get
				If _injectMethod Is Nothing Then
					_injectMethod = AddressOf Me.inject
				End If
				Return _injectMethod
			End Get
			Set(value As MocaFieldInject)
				_injectMethod = value
			End Set
		End Property

		''' <summary>
		''' ����������
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		Private Property EventDelegateInject As MocaEventDelegateInject
			Get
				Return _injectEventDelegate
			End Get
			Set(value As MocaEventDelegateInject)
				_injectEventDelegate = value
			End Set
		End Property

		''' <summary>
		''' �P������͂�ǉ�����
		''' </summary>
		''' <param name="attributeTarget">��̓^�[�Q�b�g</param>
		''' <param name="analyzer">������͋@</param>
		''' <remarks></remarks>
		Public Sub Add(ByVal attributeTarget As AttributeAnalyzerTargets, ByVal analyzer As IAttributeAnalyzer)
			If Not _analyzers.ContainsKey(attributeTarget) Then
				_analyzers.Add(attributeTarget, New List(Of IAttributeAnalyzer))
			End If

			Dim val As IList(Of IAttributeAnalyzer)
			val = _analyzers(attributeTarget)
			val.Add(analyzer)
		End Sub

		''' <summary>
		''' ��͂����O����Namespace��ǉ�����
		''' </summary>
		''' <param name="val"></param>
		''' <remarks></remarks>
		Public Sub AddIgnoreNamespace(ByVal val As String)
			If _ignoreNamespace.Contains(val) Then
				Return
			End If

			_ignoreNamespace.Add(val)
		End Sub

		''' <summary>
		''' �t�B�[���h�փC���X�^���X�̒���
		''' </summary>
		''' <param name="target">�ΏۂƂȂ�I�u�W�F�N�g</param>
		''' <param name="field">�ΏۂƂȂ�t�B�[���h</param>
		''' <param name="component">�ΏۂƂȂ�R���|�[�l���g</param>
		''' <returns>���������C���X�^���X</returns>
		''' <remarks></remarks>
		Protected Function inject(ByVal target As Object, ByVal field As FieldInfo, ByVal component As MocaComponent) As Object
			Dim instance As Object
			instance = component.Create(target)
			ClassUtil.Inject(target, field, New Object() {instance})
			Return instance
		End Function

	End Class

End Namespace
