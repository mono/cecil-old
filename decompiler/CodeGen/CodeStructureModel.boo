# roots of the known universe

abstract class Statement (ICodeNode):
	pass

abstract class Expression (ICodeNode):
	pass

[collection (Statement)]
class StatementCollection:
	pass

[collection (Expression)]
class ExpressionCollection:
	pass

# statements

class BlockStatement (Statement):
	Statements as StatementCollection = StatementCollection ()

class ReturnStatement (Statement):
	Expression as Expression

class GotoStatement (Statement):
	Label as string

class LabeledStatement (Statement):
	Label as string

class IfStatement (Statement):
	Condition as Expression
	Then as BlockStatement
	Else as BlockStatement

class ExpressionStatement (Statement):
	Expression as Expression

class ThrowStatement (Statement):
	Expression as Expression

class WhileStatement (Statement):
	Condition as Expression
	Body as BlockStatement

class DoWhileStatement (Statement):
	Condition as Expression
	Body as BlockStatement

class BreakStatement (Statement):
	pass

class ContinueStatement (Statement):
	pass

class ForStatement (Statement):
	Initializer as Statement
	Condition as Expression
	Increment as Statement
	Body as BlockStatement

class ForEachStatement (Statement):
	Variable as VariableDeclarationExpression
	Expression as Expression
	Body as BlockStatement

# switch statement fu

abstract class SwitchCase (ICodeNode):
	Body as BlockStatement

[collection (SwitchCase)]
class SwitchCaseCollection:
	pass

class ConditionCase (SwitchCase):
	Condition as Expression

class DefaultCase (SwitchCase):
	pass

class SwitchStatement (Statement):
	Expression as Expression
	Cases as SwitchCaseCollection = SwitchCaseCollection ()

# seh

class CatchClause (ICodeNode):
	Body as BlockStatement
	Type as TypeReference
	Variable as VariableDeclarationExpression

[collection (CatchClause)]
class CatchClauseCollection:
	pass

class TryStatement (Statement):
	Try as BlockStatement
	CatchClauses as CatchClauseCollection = CatchClauseCollection ()
	Fault as BlockStatement
	Finally as BlockStatement

# expressions

class BlockExpression (Expression):
	Expressions as ExpressionCollection = ExpressionCollection ()

class MethodInvocationExpression (Expression):
	Method as Expression
	Arguments as ExpressionCollection = ExpressionCollection ()

class MethodReferenceExpression (Expression):
	Target as Expression
	Method as MethodReference

class DelegateCreationExpression (Expression):
	Type as TypeReference
	Method as MethodReference
	Target as Expression

class DelegateInvocationExpression (Expression):
	Target as Expression
	Arguments as ExpressionCollection = ExpressionCollection ()

class LiteralExpression (Expression):
	Value as object

enum UnaryOperator:
	Negate
	LogicalNot
	BitwiseNot
	PostDecrement
	PostIncrement
	PreDecrement
	PreIncrement

class UnaryExpression (Expression):
	Operator as UnaryOperator
	Operand as Expression

enum BinaryOperator:
	Add
	Subtract
	Multiply
	Divide
	ValueEquality
	ValueInequality
	LogicalOr
	LogicalAnd
	LessThan
	LessThanOrEqual
	GreaterThan
	GreaterThanOrEqual
	LeftShift
	RightShift
	BitwiseOr
	BitwiseAnd
	BitwiseXor
	Modulo

class BinaryExpression (Expression):
	Operator as BinaryOperator
	Left as Expression
	Right as Expression

class AssignExpression (Expression):
	Target as Expression
	Expression as Expression

class ArgumentReferenceExpression (Expression):
	Parameter as ParameterReference

class VariableReferenceExpression (Expression):
	Variable as VariableReference

class VariableDeclarationExpression (Expression):
	Variable as VariableDefinition

class ThisReferenceExpression (Expression):
	pass

class BaseReferenceExpression (Expression):
	pass

class FieldReferenceExpression (Expression):
	Target as Expression
	Field as FieldReference

class CastExpression (Expression):
	Expression as Expression
	TargetType as TypeReference

class SafeCastExpression (Expression):
	Expression as Expression
	TargetType as TypeReference

class CanCastExpression (Expression):
	Expression as Expression
	TargetType as TypeReference

class TypeOfExpression (Expression):
	Type as TypeReference

class ConditionExpression (Expression):
	Condition as Expression
	Then as Expression
	Else as Expression

class NullCoalesceExpression (Expression):
	Condition as Expression
	Expression as Expression

class AddressDereferenceExpression (Expression):
	Expression as Expression

class AddressReferenceExpression (Expression):
	Expression as Expression

class AddressOfExpression (Expression):
	Expression as Expression

class ArrayCreationExpression (Expression):
	Type as TypeReference
	Dimensions as ExpressionCollection = ExpressionCollection ()
	Initializer as BlockExpression

class ArrayIndexerExpression (Expression):
	Target as Expression
	Indices as ExpressionCollection = ExpressionCollection ()

class ObjectCreationExpression (Expression):
	Constructor as MethodReference
	Type as TypeReference
	Arguments as ExpressionCollection = ExpressionCollection ()
	Initializer as BlockExpression

class PropertyReferenceExpression (Expression):
	Target as Expression
	Property as PropertyReference
