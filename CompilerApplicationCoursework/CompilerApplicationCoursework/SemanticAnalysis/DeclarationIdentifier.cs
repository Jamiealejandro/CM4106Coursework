using Compiler.IO;
using Compiler.Nodes;
using Compiler.Tokenization;
using System.Collections.Generic;
using System.Reflection;
using static System.Reflection.BindingFlags;

namespace Compiler.SemanticAnalysis
{

    public class DeclarationIdentifier
    {

        public ErrorReporter Reporter { get; }

        public SymbolTable SymbolTable { get; }

        public DeclarationIdentifier(ErrorReporter reporter)
        {
            Reporter = reporter;
            SymbolTable = new SymbolTable();
        }

        public void PerformIdentification(ProgramNode program)
        {
            foreach (KeyValuePair<string, IDeclarationNode> item in StandardEnvironment.GetItems())
                SymbolTable.Enter(item.Key, item.Value);
            PerformIdentificationOnProgram(program);
        }

        private void PerformIdentification(IAbstractSyntaxTreeNode node)
        {
            if (node is null)
                
                Debugger.Write("Tried to perform identification on a null tree node");
            else if (node is ErrorNode)
                
                Debugger.Write("Tried to perform identification on an error tree node");
            else
            {
                string functionName = "PerformIdentificationOn" + node.GetType().Name.Remove(node.GetType().Name.Length - 4);
                MethodInfo function = this.GetType().GetMethod(functionName, NonPublic | Public | Instance | Static);
                if (function == null)
                   
                    Debugger.Write($"Couldn't find the function {functionName} when doing identification");
                else
                    function.Invoke(this, new[] { node });
            }
        }

        private void PerformIdentificationOnProgram(ProgramNode programNode)
        {
            PerformIdentification(programNode.Command);
        }

        private void PerformIdentificationOnAssignCommand(AssignCommandNode assignCommand)
        {
            PerformIdentification(assignCommand.Identifier);
            PerformIdentification(assignCommand.Expression);
        }

        private void PerformIdentificationOnBlankCommand(BlankCommandNode blankCommand)
        {
        }

        private void PerformIdentificationOnCallCommand(CallCommandNode callCommand)
        {
            PerformIdentification(callCommand.Identifier);
            PerformIdentification(callCommand.Parameter);
        }

        private void PerformIdentificationOnIfCommand(IfCommandNode ifCommand)
        {
            PerformIdentification(ifCommand.Expression);
            PerformIdentification(ifCommand.ThenCommand);
            PerformIdentification(ifCommand.ElseCommand);
            PerformIdentification(ifCommand.NoElseCommand);
        }

        private void PerformIdentificationOnLetCommand(LetCommandNode letCommand)
        {
            SymbolTable.OpenScope();
            PerformIdentification(letCommand.Declaration);
            PerformIdentification(letCommand.Command);
            SymbolTable.CloseScope();
        }

        private void PerformIdentificationOnSequentialCommand(SequentialCommandNode sequentialCommand)
        {
            foreach (ICommandNode command in sequentialCommand.Commands)
                PerformIdentification(command);
        }

        private void PerformIdentificationOnWhileCommand(WhileCommandNode whileCommand)
        {
            PerformIdentification(whileCommand.expression);
            PerformIdentification(whileCommand.command);
            PerformIdentification(whileCommand.repeatcommand);
        }

        private void PerformIdentificationOnUntilCommand(UntilCommandNode untilCommand)
        {
            PerformIdentification(whileCommand.expression);
        }

        private void PerformIdentificationOnConstDeclaration(ConstDeclarationNode constDeclaration)
        {
            Token token = constDeclaration.Identifier.IdentifierToken;
            bool success = SymbolTable.Enter(token.Spelling, constDeclaration);
            PerformIdentification(constDeclaration.Expression);
        }

        private void PerformIdentificationOnSequentialDeclaration(SequentialDeclarationNode sequentialDeclaration)
        {
            foreach (IDeclarationNode declaration in sequentialDeclaration.Declarations)
                PerformIdentification(declaration);
        }

        private void PerformIdentificationOnVarDeclaration(VarDeclarationNode varDeclaration)
        {
            PerformIdentification(varDeclaration.TypeDenoter);
            Token token = varDeclaration.Identifier.IdentifierToken;
            bool success = SymbolTable.Enter(token.Spelling, varDeclaration);
        }

        private void PerformIdentificationOnBinaryExpression(BinaryExpressionNode binaryExpression)
        {
            PerformIdentification(binaryExpression.LeftExpression);
            PerformIdentification(binaryExpression.Op);
            PerformIdentification(binaryExpression.RightExpression);
        }
        private void PerformIdentificationOnCharacterExpression(CharacterExpressionNode characterExpression)
        {
            PerformIdentification(characterExpression.CharLit);
        }

        private void PerformIdentificationOnIdExpression(IdExpressionNode idExpression)
        {
            PerformIdentification(idExpression.Identifier);
        }

        private void PerformIdentificationOnIntegerExpression(IntegerExpressionNode integerExpression)
        {
            PerformIdentification(integerExpression.IntLit);
        }

        private void PerformIdentificationOnUnaryExpression(UnaryExpressionNode unaryExpression)
        {
            PerformIdentification(unaryExpression.Op);
            PerformIdentification(unaryExpression.Expression);
        }

        private void PerformIdentificationOnBlankParameter(BlankParameterNode blankParameter)
        {
        }


        private void PerformIdentificationOnExpressionParameter(ExpressionParameterNode expressionParameter)
        {
            PerformIdentification(expressionParameter.Expression);
        }

        private void PerformIdentificationOnVarParameter(VarParameterNode varParameter)
        {
            PerformIdentification(varParameter.Identifier);
        }


        private void PerformIdentificationOnTypeDenoter(TypeDenoterNode typeDenoter)
        {
            PerformIdentification(typeDenoter.Identifier);
        }

        private void PerformIdentificationOnCharacterLiteral(CharacterLiteralNode characterLiteral)
        {
            PerformIdentification(characterLiteral.Identifiter);
        }

        private void PerformIdentificationOnIdentifier(IdentifierNode identifier)
        {
            string name = identifier.IdentifierToken.Spelling;
            IDeclarationNode declaration = SymbolTable.Retrieve(identifier.IdentifierToken.Spelling);
            identifier.Declaration = declaration;
        }

        private void PerformIdentificationOnIntegerLiteral(IntegerLiteralNode integerLiteral)
        {
            PerformIdentification(integerLiteral.Identifier);
        }

        private void PerformIdentificationOnOperator(OperatorNode operation)
        {
            IDeclarationNode declaration = SymbolTable.Retrieve(operation.OperatorToken.Spelling);
            operation.Declaration = declaration;
        }
    }
}
