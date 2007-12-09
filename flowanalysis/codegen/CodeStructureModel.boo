abstract class Expression(ICodeElement):
	pass

abstract class Statement(ICodeElement):
	pass

[collection(Expression)]
class ExpressionCollection:
	pass

[collection(Statement)]
class StatementCollection:
	pass

class MethodInvocationExpression(Expression):
	Target as Expression
	Arguments as ExpressionCollection

class MethodReferenceExpression(Expression):
	Target as Expression
	Method as MethodReference

class LiteralExpression(Expression):
	Value as object

class UnaryExpression(Expression):
	Operator as UnaryOperator
	Operand as Expression

class BinaryExpression(Expression):
	Operator as BinaryOperator
	Left as Expression
	Right as Expression

class AssignExpression(Expression):
	Target as Expression
	Expression as Expression

class ArgumentReferenceExpression(Expression):
	Parameter as ParameterReference

class VariableReferenceExpression(Expression):
	Variable as VariableReference

class ThisReferenceExpression(Expression):
	pass

class FieldReferenceExpression(Expression):
	Target as Expression
	Field as FieldReference

class PropertyReferenceExpression(Expression):
	Target as Expression
	Property as PropertyReference

class BlockStatement(Statement):
	Statements as StatementCollection

class ReturnStatement(Statement):
	Expression as Expression

class CastExpression(Expression):
	Target as Expression
	ToType as TypeReference

class TryCastExpression(Expression):
	Target as Expression
	ToType as TypeReference
