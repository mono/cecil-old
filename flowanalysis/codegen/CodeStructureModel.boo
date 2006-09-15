abstract class Expression(ICodeElement):
	pass

abstract class Statement(ICodeElement):
	pass

[collection(IExpression)]
class ExpressionCollection:
	pass

[collection(IStatement)]
class StatementCollection:
	pass

class MethodInvocationExpression(IExpression):
	Target as Expression
	Arguments as ExpressionCollection

class MethodReferenceExpression(IExpression):
	Target as Expression
	Method as MethodReference

class LiteralExpression(IExpression):
	Value as object

class UnaryExpression(IExpression):
	Operator as UnaryOperator
	Operand as Expression

class BinaryExpression(IExpression):
	Operator as BinaryOperator
	Left as Expression
	Right as Expression

class AssignExpression(IExpression):
	Target as Expression
	Expression as Expression

class ArgumentReferenceExpression(IExpression):
	Parameter as ParameterReference

class VariableReferenceExpression(IExpression):
	Variable as VariableReference

class ThisReferenceExpression(IExpression):
	pass

class FieldReferenceExpression(IExpression):
	Target as Expression
	Field as FieldReference

class PropertyReferenceExpression(IExpression):
	Target as Expression
	Property as PropertyReference

class BlockStatement(IStatement):
	Statements as StatementCollection

class ReturnStatement(IStatement):
	Expression as Expression
