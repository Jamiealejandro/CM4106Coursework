using Compiler.IO;
using Compiler.Nodes;
using System.Reflection;
using static System.Reflection.BindingFlags;

namespace Compiler.SemanticAnalysis
{
    
    public class TypeChecker
    {
        
        public ErrorReporter Reporter { get; }

        public TypeChecker(ErrorReporter reporter)
        {
            Reporter = reporter;
        }

        public void PerformTypeChecking(ProgramNode tree)
        {
            PerformTypeCheckingOnProgram(tree);
        }

        private void PerformTypeChecking(IAbstractSyntaxTreeNode node)
        {
            if (node is null)
                
                Debugger.Write("Tried to perform type checking on a null tree node");
            else if (node is ErrorNode)
               
                Debugger.Write("Tried to perform type checking on an error tree node");
            else
            {
                string functionName = "PerformTypeCheckingOn" + node.GetType().Name.Remove(node.GetType().Name.Length - 4);
                MethodInfo function = this.GetType().GetMethod(functionName, NonPublic | Public | Instance | Static);
                if (function == null)
                   
                    Debugger.Write($"Couldn't find the function {functionName} when type checking");
                else
                    function.Invoke(this, new[] { node });
            }
        }

        private void PerformTypeCheckingOnProgram(ProgramNode programNode)
        {
            PerformTypeChecking(programNode.Command);
        }

        private void PerformTypeCheckingOnAssignCommand(AssignCommandNode assignCommand)
        {
            PerformTypeChecking(assignCommand.Identifier);
            PerformTypeChecking(assignCommand.Expression);
            if (!(assignCommand.Identifier.Declaration is IVariableDeclarationNode varDeclaration))
            {
                ErrorCount++;
            }
            else if (varDeclaration.EntityType != assignCommand.Expression.Type)
            {
                ErrorCount++;
            }
        }

        
        private void PerformTypeCheckingOnBlankCommand(BlankCommandNode blankCommand)
        {
        }

        private void PerformTypeCheckingOnCallCommand(CallCommandNode callCommand)
        {
            PerformTypeChecking(callCommand.Identifier);
            PerformTypeChecking(callCommand.Parameter);
            if (!(callCommand.Identifier.Declaration is FunctionDeclarationNode functionDeclaration))
            {
                ErrorCount++;
            }
            else if (GetNumberOfArguments(functionDeclaration.Type) == 0)
            {
                if (!(callCommand.Parameter is BlankParameterNode))
                {
                    ErrorCount++;
                    ReportError();
                }
            }
            else
            {
                if (callCommand.Parameter is BlankParameterNode)
                {
                    ErrorCount++;
                    ReportError();
                }
                else
                {
                    if (GetArgumentType(functionDeclaration.Type, 0) != callCommand.Parameter.Type)
                    {
                        ErrorCount++;
                        ReportError();
                    }
                    if (ArgumentPassedByReference(functionDeclaration.Type, 0))
                    {
                        if (!(callCommand.Parameter is VarParameterNode))
                        {
                            ErrorCount++;
                            ReportError();
                        }
                    }
                    else
                    {
                        if (!(callCommand.Parameter is ExpressionParameterNode))
                        {
                            ErrorCount++;
                            ReportError();
                        }
                    }
                }
            }
        }

        private void PerformTypeCheckingOnUntilCommand(UntilCommandNode untilCommand)
        {
            PerformTypeChecking(untilCommand.Command);
        }
        
        private void PerformTypeCheckingOnIfCommand(IfCommandNode ifCommand)
        {
            PerformTypeChecking(ifCommand.Expression);
            PerformTypeChecking(ifCommand.ThenCommand);
            PerformTypeChecking(ifCommand.ElseCommand);
            PerformTypeChecking(ifCommand.NoElseCommand);
            if (ifCommand.Expression.Type != StandardEnvironment.BooleanType)
            {
                ErrorCount++;
                ReportError();
            }
        }

        
        private void PerformTypeCheckingOnLetCommand(LetCommandNode letCommand)
        {
            PerformTypeChecking(letCommand.Declaration);
            PerformTypeChecking(letCommand.Command);
        }

        
        private void PerformTypeCheckingOnSequentialCommand(SequentialCommandNode sequentialCommand)
        {
            foreach (ICommandNode command in sequentialCommand.Commands)
                PerformTypeChecking(command);
        }

        private void PerformTypeCheckingOnWhileCommand(WhileCommandNode whileCommand)
        {
            PerformTypeChecking(whileCommand.Expression);
            PerformTypeChecking(whileCommand.Command);
            PerformTypeChecking(whileCommand.repeatCommand);
            if (whileCommand.Expression.Type != StandardEnvironment.BooleanType)
            {
                ErrorCount++;
                ReportError();
            }
        }

        private void PerformTypeCheckingOnConstDeclaration(ConstDeclarationNode constDeclaration)
        {
            PerformTypeChecking(constDeclaration.Identifier);
            PerformTypeChecking(constDeclaration.Expression);
        }

     
        private void PerformTypeCheckingOnSequentialDeclaration(SequentialDeclarationNode sequentialDeclaration)
        {
            foreach (IDeclarationNode declaration in sequentialDeclaration.Declarations)
                PerformTypeChecking(declaration);
        }

        private void PerformTypeCheckingOnVarDeclaration(VarDeclarationNode varDeclaration)
        {
            PerformTypeChecking(varDeclaration.TypeDenoter);
            PerformTypeChecking(varDeclaration.Identifier);
        }


        private void PerformTypeCheckingOnBinaryExpression(BinaryExpressionNode binaryExpression)
        {
            PerformTypeChecking(binaryExpression.Op);
            PerformTypeChecking(binaryExpression.LeftExpression);
            PerformTypeChecking(binaryExpression.RightExpression);
            if (!(binaryExpression.Op.Declaration is BinaryOperationDeclarationNode opDeclaration))
            {
                ErrorCount++;
                ReportError();
            }
            else
            {
                if (GetArgumentType(opDeclaration.Type, 0) == StandardEnvironment.AnyType)
                {
                    if (binaryExpression.LeftExpression.Type != binaryExpression.RightExpression.Type)
                    {
                        ErrorCount++;
                        ReportError();
                    }
                }
                else
                {
                    if (GetArgumentType(opDeclaration.Type, 0) != binaryExpression.LeftExpression.Type)
                    {
                        ErrorCount++;
                        ReportError();
                    }
                    if (GetArgumentType(opDeclaration.Type, 1) != binaryExpression.RightExpression.Type)
                    {
                        ErrorCount++;
                        ReportError();
                    }
                }
                binaryExpression.Type = GetReturnType(opDeclaration.Type);
            }
        }

        private void PerformTypeCheckingOnCharacterExpression(CharacterExpressionNode characterExpression)
        {
            PerformTypeChecking(characterExpression.CharLit);
            characterExpression.Type = StandardEnvironment.CharType;
        }

        private void PerformTypeCheckingOnIdExpression(IdExpressionNode idExpression)
        {
            PerformTypeChecking(idExpression.Identifier);
            if (!(idExpression.Identifier.Declaration is IEntityDeclarationNode declaration))
            {
                ErrorCount++;
                ReportError();
            }
            else
                idExpression.Type = declaration.EntityType;
        }

       
        private void PerformTypeCheckingOnIntegerExpression(IntegerExpressionNode integerExpression)
        {
            PerformTypeChecking(integerExpression.IntLit);
            integerExpression.Type = StandardEnvironment.IntegerType;
        }

        
        private void PerformTypeCheckingOnUnaryExpression(UnaryExpressionNode unaryExpression)
        {
            PerformTypeChecking(unaryExpression.Op);
            PerformTypeChecking(unaryExpression.Expression);
            if (!(unaryExpression.Op.Declaration is UnaryOperationDeclarationNode opDeclaration))
            {
                ErrorCount++;
                ReportError();
            }
            else
            {
                if (GetArgumentType(opDeclaration.Type, 0) != unaryExpression.Expression.Type)
                {
                    ErrorCount++;
                    ReportError();
                }
                unaryExpression.Type = GetReturnType(opDeclaration.Type);
            }
        }


        private void PerformTypeCheckingOnBlankParameter(BlankParameterNode blankParameter)
        {
        }

        private void PerformTypeCheckingOnExpressionParameter(ExpressionParameterNode expressionParameter)
        {
            PerformTypeChecking(expressionParameter.Expression);
            expressionParameter.Type = expressionParameter.Expression.Type;
        }

        private void PerformTypeCheckingOnVarParameter(VarParameterNode varParameter)
        {
            PerformTypeChecking(varParameter.Identifier);
            if (!(varParameter.Identifier.Declaration is IVariableDeclarationNode varDeclaration))
            {
                ErrorCount++;
                ReportError();
            }
            else
                varParameter.Type = varDeclaration.EntityType;
        }


        private void PerformTypeCheckingOnTypeDenoter(TypeDenoterNode typeDenoter)
        {
            PerformTypeChecking(typeDenoter.Identifier);
            if (!(typeDenoter.Identifier.Declaration is SimpleTypeDeclarationNode declaration))
            {
                ErrorCount++;
                ReportError();
            }
            else
                typeDenoter.Type = declaration;
        }

        private void PerformTypeCheckingOnCharacterLiteral(CharacterLiteralNode characterLiteral)
        {
            if (characterLiteral.Value < short.MinValue || characterLiteral.Value > short.MaxValue)
            {
                ErrorCount++;
                ReportError();
            }
        }

        private void PerformTypeCheckingOnIdentifier(IdentifierNode identifier)
        {
        }

        
        private void PerformTypeCheckingOnIntegerLiteral(IntegerLiteralNode integerLiteral)
        {
            if (integerLiteral.Value < short.MinValue || integerLiteral.Value > short.MaxValue)
            {
                ErrorCount++;
                ReportError();
            }
        }

        private void PerformTypeCheckingOnOperator(OperatorNode operation)
        {
        }

        private static int GetNumberOfArguments(FunctionTypeDeclarationNode node)
        {
            return node.Parameters.Length;
        }

       
        private static SimpleTypeDeclarationNode GetArgumentType(FunctionTypeDeclarationNode node, int argument)
        {
            return node.Parameters[argument].type;
        }

        private static bool ArgumentPassedByReference(FunctionTypeDeclarationNode node, int argument)
        {
            return node.Parameters[argument].byRef;
        }

        
        private static SimpleTypeDeclarationNode GetReturnType(FunctionTypeDeclarationNode node)
        {
            return node.ReturnType;
        }
    }
}
