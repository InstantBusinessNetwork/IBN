using System;
using System.IO;
using System.Text;
using System.Collections;

namespace Mediachase.IBN.Business.SpreadSheet
{
	public class ExpressionParser
	{
		private SpreadSheetView view = null;

		private static Hashtable ops, trees, spconst;
        private static int maxoplength;
        private static int sb_init;

		/// <summary>
		/// Default constructor, creates an ExpressionParser object
		/// </summary>
        public ExpressionParser()
        {
        }

		static ExpressionParser()
		{
			ops   = Hashtable.Synchronized( new Hashtable( 52 )); // Holds operators
			spconst = Hashtable.Synchronized(new Hashtable( 12 )); // Holds constants
			trees = Hashtable.Synchronized(new Hashtable( 101 )); // Holds Node tree datastructures
			
			// Add all valid operators.
			// new Operator( operator, arguments, precedence )
			// 
			// To add a new operator to the parser three things need to be added:
			//
			// 1. A new line below specifying the operator symbol, the number of
			// arguments (in this parser max two arguments!) and the operator precedence.
			//
			// 2. Change the maxoplength below (if needed) to hold the number of characters
			// an operator symbol length can be.
			//
			// 3. Add the code to evaluate the operator inside the toValue method using the
			// same recursive calls as for the other operators.
			//
			ops.Add( "^",     new Operator( "^",	2, 3 ) );
			ops.Add( "+",     new Operator( "+",	2, 6 ) );
			ops.Add( "-",     new Operator( "-",	2, 6 ) );
			ops.Add( "/", 	  new Operator( "/",	2, 4 ) );
			ops.Add( "*",     new Operator( "*",	2, 4 ) );
			ops.Add( "cos",   new Operator( "cos",	1, 2 ) );
			ops.Add( "sin",   new Operator( "sin",	1, 2 ) );
			ops.Add( "exp",   new Operator( "exp",	1, 2 ) );
			ops.Add( "ln",    new Operator( "ln",	1, 2 ) );
			ops.Add( "tan",   new Operator( "tan",	1, 2 ) );
			ops.Add( "acos",  new Operator( "acos",	1, 2 ) );
			ops.Add( "asin",  new Operator( "asin",	1, 2 ) );
			ops.Add( "atan",  new Operator( "atan",	1, 2 ) );
			ops.Add( "cosh",  new Operator( "cosh",	1, 2 ) );
			ops.Add( "sinh",  new Operator( "sinh",	1, 2 ) );
			ops.Add( "tanh",  new Operator( "tanh",	1, 2 ) );
			ops.Add( "sqrt",  new Operator( "sqrt",	1, 2 ) );
			ops.Add( "cotan", new Operator( "cotan",1, 2 ) );
			ops.Add( "fpart", new Operator( "fpart",1, 2 ) );
			ops.Add( "acotan",new Operator( "acotan",1, 2 ) );
			ops.Add( "round", new Operator( "round", 1, 2 ) );
			ops.Add( "ceil",  new Operator( "ceil",  1, 2 ) );
			ops.Add( "floor", new Operator( "floor",1, 2 ) );
			ops.Add( "fac",	  new Operator( "fac",	1, 2 ) );
			ops.Add( "sfac",  new Operator( "sfac",	1, 2 ) );
			ops.Add( "abs",	  new Operator( "abs",	1, 2 ) );
			ops.Add( "log",	  new Operator( "log",	2, 5 ) );
			ops.Add( "%",     new Operator( "%",	2, 4 ) );
			ops.Add( ">",     new Operator( ">",	2, 7 ) );
			ops.Add( "<",     new Operator( "<",	2, 7 ) );
			ops.Add( "&&",    new Operator( "&&",	2, 10) );
			ops.Add( "==",    new Operator( "==",	2, 8 ) );
			ops.Add( "!=",    new Operator( "!=",	2, 8 ) );
			ops.Add( "||",    new Operator( "||",	2, 11 ) );
			ops.Add( "!",     new Operator( "!",	1, 1 ) );
			ops.Add( ">=",    new Operator( ">=",	2, 7 ) );
			ops.Add( "<=",    new Operator( "<=" ,	2, 7 ) );
			
			// Constants
			spconst.Add( "euler",     Math.E  );
			spconst.Add( "pi" ,       Math.PI );
			spconst.Add( "nan" ,      double.NaN );
			spconst.Add( "infinity" , double.PositiveInfinity );
			spconst.Add( "true" ,    1D  );
			spconst.Add( "false",    0D );
					
			// maximum operator length, used when parsing.
			maxoplength = 6;
			
			// init all StringBuilders with this value.
			// this will be set to the length of the expression being evaluated by Parse.
			sb_init = 256;
		}

		private static double DoubleParse(string Value)
		{
			double dblValue = 0;

			try
			{
				dblValue = double.Parse(Value);
			}
			catch
			{
				dblValue = double.Parse(Value, System.Globalization.CultureInfo.InvariantCulture);
			}

			return dblValue;
		}


		/// <summary>Matches all paranthesis and returns true if they all match or false if they do not.</summary>
		/// <param name="exp">expression to check, infix notation</param>
		/// <returns>true if ok false otherwise</returns>
		private bool matchParant( String exp )
		{
			int count = 0;
			int i = 0;
		
			int l = exp.Length;
		
			for(i = 0; i < l ; i++ )
			{
				if( exp[i] == '(' )
				{
					count++;
				}
				else if( exp[i] == ')' )
				{
					count--;
				}
			}
		
			return( count == 0 );
		}
		


		/// <summary>Checks if the character is alphabetic.</summary>
		/// <param name="ch">Character to check</param>
		/// <returns>true or false</returns>
		private bool isAlpha( char ch )
		{
			return( (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z') || ch=='[' || ch==']' || ch=='_' || ch==':' );
		}

		/// <summary>Checks if the string can be considered to be a valid variable name.</summary>
		/// <param name="str">The String to check</param>
		/// <returns>true or false</returns>
		private bool isVariable( String str )
		{
			int i = 0;
			int len = str.Length;
		
			if( isAllNumbers( str ) ) return false;
		
			for( i = 0; i < len ; i++  )
			{
				if( getOp( str , i ) != null || isAllowedSym( str[i])  )
				{
					return false;
				} 
			}
		
			return true;
		}
		

		/// <summary>Checks if the character is a digit</summary>
		/// <param name="ch">Character to check</param>
		/// <returns>true or false</returns>
		private bool isConstant( char ch )
		{
			return( Char.IsDigit( ch ) );
		}



		/// <summary>Checks to se if a string is numeric</summary>
		/// <param name="exp">String to check</param>
		/// <returns>true if the string was numeric, false otherwise</returns>
		private bool isConstant( String exp )
		{ 
			try
			{  
				if( Double.IsNaN( DoubleParse( exp ) ) )
				{
					return false;
				}
			}
			catch
			{
				return false;
			}
		  
			return true;
		}



		/// <summary>
		/// Checks to see if this String consists of only digits and punctuation.
		/// </summary>
		/// <remarks>
		/// NOTE: needed in .NET at all ? This is a legacy from the Java version
		/// where it was needed because some older JVM's accepted strings that started
		/// with digits as numeric when the isConstant method was used.
		/// </remarks>
		/// <param name="str">The string to check</param>
		/// <returns>true if the string was numeric, false otherwise.</returns>
		private bool isAllNumbers( String str )
		{
			char ch;
			int i = 0, l = 0;
			bool dot = false;
		    
			ch = str[0];
		    
			if(  ch == '-' || ch == '+'  ) i = 1;
		   
			l = str.Length;
		
			while( i < l  )
			{
				ch = str[i];

				if( ! ( Char.IsDigit( ch ) || ( ( ch == '.' || ch == ',') && ! dot ) ) ) 
				{
					return false;
				}
		  
				dot = (  ch == '.' || ch == ','  );

				i++;
			}
		    
			return true;
		}
		


		/// <summary>
		/// Checks to see if the string is the name of a acceptable operator.
		/// </summary>
		/// <param name="str">The string to check</param>
		/// <returns>true if it is an acceptable operator, false otherwise.</returns>
		private bool isOperator( String str )
		{
			return( ops.ContainsKey( str ) );
		}



		/// <summary>
		/// Checks to see if the operator name represented by str takes two arguments.
		/// </summary>
		/// <param name="str">The string to check</param>
		/// <returns>true if the operator takes two arguments, false otherwise.</returns>
		private bool isTwoArgOp( String str )
		{
			if( str == null ) return false;
			Object o = ops[ str ];
			if( o == null ) return false;
			return( ((Operator)o).arguments() == 2 );
		}
		

		/// <summary>
		/// Checks to see if the double value a can be considered to be a mathematical integer.</summary>
		/// <remarks>
		/// This method is only used by the fac and sfac methods and not the parser itself, it should
		/// really leave this class since they have nothing to do with the parser.
		/// </remarks>
		/// <param name="a">the double value to check</param>
		/// <returns>true if the double value is an integer, false otherwise.</returns>
		private bool isInteger( double a  )
		{
			return( ( a - (int)a) == 0.0 );
		}
		
		
		/// <summary>
		/// Checks to see if the int value a can be considered to be even. </summary>
		/// <remarks>
		/// This method is only used by the fac and sfac methods and not the parser itself, it should
		/// really leave this class since they have nothing to do with the parser.
		/// </remarks>
		/// <param name="a">the int value to check</param>
		/// <returns>true if the int value is even, false otherwise.</returns>
		private bool isEven( int a )
		{
			return( isInteger( a / 2 ) );
		}
		

		/// <summary>
		/// Checks to see if the character is a valid symbol for this parser.
		/// </summary>
		/// <param name="s">the character to check</param>
		/// <returns>true if the char is valid, false otherwise.</returns>
		private bool isAllowedSym( char s )
		{	
			return( s == ',' || s == '.' || s == ')' || s == '(' || s == '>' || s == '<' || s == '&' || s == '=' || s == '|'); 
		}



		/// <summary>
		/// Checks the String expression to see if the syntax is valid.
		/// this method doesn't return anything, instead it throws an Exception
		/// if the syntax is invalid.
		///	
		/// Examples of invalid syntax can be non matching paranthesis, non valid symbols appearing
		/// or a variable or operator name is invalid in the expression.
		/// </summary>
		/// <param name="exp">the string expression to check, infix notation.</param>
		private void Syntax( String exp )
		{
			int i = 0, oplen = 0;
			String op = null;
			String nop = null;  
		
			if( ! matchParant(exp) )
			{
				throw new System.Exception("Non matching paranthesis");
			}
		
			int l = exp.Length;

			bool bSkipVar = false;
		
			while( i < l )
			{
				try
				{
					if(bSkipVar)
					{
						if(exp[i]==']')
							bSkipVar = false;
						i++;
					}
					else 
					{
						if(exp[i]=='[')
						{
							bSkipVar = true;
							i++;
						}
						else if( ( op = getOp( exp , i ) ) != null)
						{
							oplen = op.Length;
							i += oplen;
							nop = getOp( exp, i );
							if( nop != null && isTwoArgOp( nop ) && ! ( nop.Equals("+") || nop.Equals("-") ) )
							{
								throw new System.Exception( "Syntax error near -> " + exp.Substring( i - oplen ) );
							}
						}
						else if( ! isAlpha( exp[i] ) && ! isConstant( exp[i] ) && ! isAllowedSym( exp[i] ) )
						{
							throw new System.Exception( "Syntax error near -> " + exp.Substring( i ) );
						}
						else
						{
							i++;
						}
					}
				}	
				catch( IndexOutOfRangeException )
				{
					i++;
				}
			}
		
			return;
		}
		


		/// <summary>
		/// Inserts the multiplication operator where needed.
		/// This method adds limited juxtapositioning support.
		/// </summary>
		/// <remarks>	
		/// Juxtaposition is supported in these type cases:
		///
		/// case: variable jp one-arg-op , xcos(x)
		/// case: const jp variable or one-arg-op, 2x, 2tan(x)
		/// case: "const jp ( expr )" , 2(3+x)
		/// case: ( expr ) jp variable or one-arg-op , (2-x)x , (2-x)sin(x)
		/// case: var jp  ( expr ) , x(x+1) , x(1-sin(x))
		///
		/// Note that this also puts extra limitations on variable names, they cannot
		/// contain digits within them or at the beginning, only at the end.
		/// </remarks>
		/// <param name="exp">the infix string expression to process</param>
		/// <returns>the processed infix expression</returns>
		private String putMult( String exp )
		{
			int i = 0, p = 0;
			String tmp = null;
			StringBuilder str = new StringBuilder( exp );
		
			int l = exp.Length;
		
			while( i < l )
			{	
				try
				{
		
					if( ( tmp = getOp( exp , i ) ) != null && ! isTwoArgOp( tmp ) && isAlpha( exp[ i - 1 ] ) )  
					{
						// case: variable jp one-arg-op , xcos(x)
						str.Insert( i + p , "*" );
						p++;
					} 								
					else if( isAlpha( exp[i] ) && isConstant( exp[ i - 1 ] ) 
						&& ( tmp == null || ! tmp.Equals( "log" ) ) ) // tmp was set by previous test
					{
			
						// case: const jp variable or one-arg-op, 2x, 2tan(x)
						// note that "log" is treated specially
						str.Insert( i + p , "*" );
						p++;
					}
					else if( exp[i] == '(' && isConstant( exp[ i - 1 ] ) )
					{  
						// case: "const jp ( expr )" , 2(3+x)
						str.Insert( i + p , "*" );
						p++;
					}
					else if( isAlpha( exp[i] ) && exp[ i - 1 ] == ')' 
						&& ( tmp == null || ! tmp.Equals( "log" ) ))  // tmp was set by previous test
					{ 
						// case: ( expr ) jp variable or one-arg-op , (2-x)x , (2-x)sin(x)
						str.Insert( i + p , "*" );
						p++;
					}      
					else if( exp[i] == '('  && exp[ i - 1 ] == ')' ) 
					{ 
						// case: ( expr ) jp  ( expr ) , (2-x)(x+1) , sin(x)(2-x) 
						str.Insert( i + p , "*" ); 
						p++; 
					}
					else if( exp[i] == '('  && isAlpha( exp[ i - 1 ] ) && backTrack( exp.Substring( 0 , i ) ) == null ) 
					{ 
						// case: var jp  ( expr ) , x(x+1) , x(1-sin(x))
						str.Insert( i + p , "*" ); 
						p++; 
					}
				}
				catch{}
		
				if( tmp != null ) 
				{
					i += tmp.Length;
				}
				else
				{
					i++;
				}
		
				tmp = null;  
			}
		
			return str.ToString();
		}



		/// <summary>
		/// Adds support for "scientific notation" by replacing the E operator with *10^
		/// </summary>
		/// <remarks>
		/// For example the value 1E-3 would be changed to 1*10^-3 which the parser will treat
		/// as a normal expression.
		/// </remarks>
		/// <param name="exp">the infix string expression to process</param>
		/// <returns>the processed infix expression</returns>
		private String parseE( String exp )
		{

		 
			int i, p , len;
		
			StringBuilder newstr = new StringBuilder( exp );
		
			i = p = 0;
			len = exp.Length;

			bool bSkipVar = false;
		
			while( i < len )
			{
				try
				{
					if(bSkipVar)
					{
						if(exp[i]==']')
							bSkipVar = false;
						i++;
					}
					else 
					{
						if(exp[i]=='[')
						{
							bSkipVar = true;
							i++;
						}
						else if( (exp[i] == 'e' || exp[i] == 'E') && Char.IsDigit( exp[ i - 1 ] ) )
						{
							if( Char.IsDigit( exp[ i + 1 ] ) || ( ( exp[ i + 1 ] == '-' || exp[ i + 1 ] == '+' ) && Char.IsDigit( exp[ i + 2 ] ) ) )
							{
								// replace the 'e'
								newstr[ i + p ] =  '*';
								// insert the rest
								newstr.Insert( i + p + 1 , "10^" );
								p = p + 3; // buffer growed by 3 chars
							}
						}
					}
				}
				catch{}
				i++;
			}
		  
			return newstr.ToString();
		}
		

		/// <summary>
		/// Parses out spaces from a string
		/// </summary>
		/// <param name="str">The string to process</param>
		/// <returns>A copy of the string stripped of all spaces</returns>
		private String skipSpaces( String str  )
		{
			int i = 0;
			int len = str.Length;
			StringBuilder nstr = new StringBuilder( len );
		  
			while( i < len )
			{
				if( str[ i ] != ' ' )
				{
					nstr.Append( str[i] );
				}
				i++;
			}
		
			return nstr.ToString();
		}
		


		/// <summary>
		/// Matches an opening left paranthesis.
		/// </summary>
		/// <param name="exp">the string to search in</param>
		/// <param name="index">the index of the opening left paranthesis</param>
		/// <returns>the index of the matching closing right paranthesis</returns>
		private int match( String exp,int index )
		{
			int len = exp.Length;
			int i = index;
			int count = 0;
		
			while( i < len )
			{
				if( exp[i] == '(')
				{
					count++;
				}
				else if( exp[i] == ')')
				{
					count--;
				}
		
				if( count == 0 ) return i;
		
				i++;
			}
		  
			return index;
		}
		


		/// <summary>
		/// Parses out an operator from an infix string expression.
		/// </summary>
		/// <param name="exp">the infix string expression to look in</param>
		/// <param name="index">the index to start searching from</param>
		/// <returns>the operator if any or null.</returns>
		private String getOp( String exp , int index )
		{
			// OZ: Check open [
			int openSquareBracket = index>0?exp.LastIndexOf('[', index-1):-1;
			int closeSquareBracket = index>0?exp.LastIndexOf(']', index-1):-1;

			if(openSquareBracket>closeSquareBracket)
				return null;

			String tmp; 
			int i = 0;
			int len = exp.Length;

			for( i = 0 ; i < maxoplength ; i++ )
			{
				if( index >= 0 && ( index + maxoplength - i ) <= len )
				{
					tmp = exp.Substring( index ,  maxoplength - i  );
					if( isOperator( tmp ) )
					{
						return( tmp );
					}
				}
			}
		
			return null;
		}

		/// <summary>
		/// Parses an infix String expression and creates a parse tree of Node's.
		/// </summary>
		/// <remarks>
		/// This is the heart of the parser, it takes a normal expression and creates
		/// a datastructure we can easily recurse when evaluating.
		///
		/// The datastructure is then evaluated by the toValue method.
		/// </remarks>
		/// <param name="exp">the infix string expression to process</param>
		/// <returns>A tree datastructure of Node objects representing the expression</returns>
		private Node parse( String exp ) 
		{
			int i , ma , len;
			String farg , sarg , fop;
			Node tree = null;
		  
			farg = sarg = fop = "";
			ma = i = 0;
		
			len = exp.Length;
		
			if( len == 0  )
			{
				throw new System.Exception("Wrong number of arguments to operator");
			}
			else if( exp[ 0 ] == '(' && (( ma = match( exp , 0 ) ) == ( len - 1 ) ) )
			{
				return( parse( exp.Substring( 1 , ma - 1 ) ));
			}
			else if( isVariable( exp ) )
			{
				return( new Node( exp ) );
			}
			else if( isAllNumbers( exp ) )  // this is really the only place where isAllNumbers matters. 
			{
				try
				{
					return( new Node( DoubleParse( exp ) ) );	
				}
				catch( FormatException )
				{
					throw new System.Exception( "Syntax error-> " + exp + " (not using regional decimal separator?)" );
				}
			}
		 
			while( i < len )
			{
				if( ( fop = getOp( exp , i ) ) == null )
				{
					farg = arg( null, exp, i );
					fop  = getOp( exp, i + farg.Length );
			
					if( fop == null ) throw new Exception( "Missing operator" );
				
					if( isTwoArgOp( fop ) )
					{
						sarg = arg( fop , exp, i + farg.Length + fop.Length );
						if( sarg.Equals( "" ) ) throw new Exception( "Wrong number of arguments to operator " + fop );
						tree = new Node( fop , parse( farg ) , parse( sarg ) );
						i += farg.Length + fop.Length + sarg.Length;
					}
					else
					{
						if( farg.Equals( "" ) ) throw new Exception( "Wrong number of arguments to operator " + fop );
						tree = new Node( fop , parse( farg ) );
						i += farg.Length + fop.Length;
					}
				
				}
				else
				{		
					if( isTwoArgOp( fop ) )
					{
						farg = arg( fop, exp, i + fop.Length );	
						if( farg.Equals( "" ) ) throw new Exception( "Wrong number of arguments to operator " + fop );
						if( tree == null )
						{
							if( fop.Equals( "+" ) || fop.Equals( "-" ) )
							{ 
								tree = new Node( 0D );
							}
							else
							{
								throw new Exception( "Wrong number of arguments to operator " + fop );	
							}
						}
						tree = new Node( fop, tree, parse( farg ) );
						i += farg.Length + fop.Length;	
					

					}
					else
					{
						farg = arg( fop, exp, i + fop.Length );
						if( farg.Equals( "" ) ) throw new Exception( "Wrong number of arguments to operator " + fop );
						tree = new Node( fop , parse( farg ) );
						i += farg.Length + fop.Length;
					

					} 	
				}

			}
		
			return tree;
		}
		

		/// <summary>
		/// Parses the infix expression for arguments to the specified operator.
		/// </summary>
		/// <param name="_operator">the operator we are interested in</param>
		/// <param name="exp">the infix string expression</param>
		/// <param name="index">the index to start the search from</param>
		/// <returns>the argument to the operator</returns>
		private String arg( String _operator, String exp, int index )
		{
			int ma, i, prec = -1;
			int len = exp.Length;
			String op = null;
		 
			StringBuilder str = new StringBuilder( sb_init );
		
			i = index;
			ma = 0;
		
			if( _operator == null )
			{
				prec = -1;
			}
			else
			{
				prec = ((Operator)ops[ _operator ]).precedence();	
			}

			while( i < len )
			{
			  
				if( exp[i] == '(')
				{
					ma = match( exp, i );
					str.Append( exp.Substring( i , ma + 1 - i ));
					i = ma + 1;
				}
				else if( ( op = getOp( exp, i )) != null )
				{
					// (_operator != null && _operator.Equals("&&") && op.Equals("||") ) || 
					if( str.Length != 0 && ! isTwoArgOp( backTrack( str.ToString() )) && ((Operator)ops[ op ]).precedence() >= prec )
					{
						return str.ToString();
					}
					str.Append( op );
					i += op.Length;
				}
				else
				{
					str.Append( exp[i] );
					i++;
				}
			}

			return str.ToString();
		}
			


		/// <summary>
		/// Returns an operator at the end of the String str if present.
		/// </summary>
		/// <remarks>
		/// Used when parsing for arguments, the purpose is to recognize
		/// expressions like for example 10^-1
		/// </remarks>
		/// <param name="str">part of infix string expression to search</param>
		/// <returns>the operator if found or null otherwise</returns>
		private String backTrack( String str  )
		{
			int i = 0;
			int len = str.Length;
			String op = null;
		
			try
			{
				for( i = 0; i <= maxoplength ; i++  )
				{
					if( ( op = getOp( str , ( len - 1 - maxoplength + i ))) != null 
						&& ( len - maxoplength - 1 + i + op.Length ) == len )
					{
						return op;
					}
				}
			}
			catch{}
		
			return null;
		}


		/// <summary>
		/// Calculates the faculty.
		/// </summary>
		/// <remarks>
		/// This method should move out of this class since it has nothing to do with the parser.
		/// it's here because the language math functions do not include faculty calculations.
		/// </remarks>
		/// <param name="val">the value to calcualte the faculty of</param>
		/// <returns>the faculty</returns>
		private double fac( double val )
		{
		
			if( ! isInteger( val ) )
			{
				return Double.NaN;
			}
			else if(  val < 0  )
			{
				return Double.NaN;
			}
			else if(  val <= 1  )
			{
				return 1;
			}
		
			return( val * fac( val - 1));
		}


		/// <summary>
		/// Calculates the semi faculty.
		/// </summary>
		/// <remarks>
		/// This method should move out of this class since it has nothing to do with the parser.
		/// it's here because the language math functions do not include semi faculty calculations.
		/// </remarks>
		/// <param name="val">the value to calcualte the semi faculty of</param>
		/// <returns>the semi faculty</returns>
		private double sfac(double val )
		{
		
			if(  ! isInteger( val ) )
			{
				return Double.NaN;
			}
			else if(  val < 0  )
			{
				return Double.NaN;
			}
			else if(  val <= 1  )
			{
				return 1;
			}
		
			return( val * sfac( val - 2));
		}



		/// <summary>
		/// Returns the decimal part of the value
		/// </summary>
		/// <param name="val">the value to calculate the fpart for</param>
		/// <returns>the decimal part of the value</returns>
		private double fpart(double val )
		{
			if(  val >= 0  )
			{
				return(val - Math.Floor(val));
			}
			else
			{
				return(val - Math.Ceiling(val));
			}
		}



		/// <summary>
		/// Parses the datastructure created by the parse method.
		/// </summary>
		/// <remarks>
		/// This is where the actual evaluation of the expression is made,
		/// the Node tree structure created by the parse method is recursed and evaluated
		/// to a double value.
		/// </remarks>
		/// <param name="tree">A Node representing a tree datastructure</param>
		/// <returns>A double value</returns>
		private double toValue( Node tree ) 
		{
			Node arg1, arg2;
			double val;
			String op, tmp;
		  
			if( tree.getType() == Node.TYPE_CONSTANT )
			{
				return( tree.getValue() );	
			}
			else if( tree.getType() == Node.TYPE_VARIABLE )
			{
				tmp = tree.getVariable();
			
				// check if PI, Euler....etc
				if( spconst.ContainsKey( tmp ) )
				{
					return( (double)spconst[ tmp ] );
				}
				
				// normal variable, get value
				return GetVariable( tmp );

//				if( isConstant( tmp ) )
//				{
//					return( Double.Parse( tmp ) );
//				}
//				else
//				{
//					Syntax( tmp );
//					return( toValue( parse( putMult( parseE( tmp ) ) ) ) ); 	
//				}
			}
		  
			op   = tree.getOperator();
			arg1 = tree.arg1();
		  
			if( tree.arguments() == 2 )
			{
				arg2 = tree.arg2();
		  
				if( op.Equals( "+" ) )
					return( toValue(arg1) + toValue(arg2));
				else if( op.Equals( "-" ) )
					return( toValue(arg1) - toValue(arg2));
				else if( op.Equals( "*" ) )
					return( toValue(arg1) * toValue(arg2));
				else if( op.Equals( "/" ) )
					return( toValue(arg1) / toValue(arg2));
				else if( op.Equals( "^" ) )
					return( Math.Pow(toValue(arg1),toValue(arg2)));
				else if( op.Equals( "log" ) )
					return( Math.Log(toValue(arg2)) / Math.Log(toValue(arg1)) );
				else if( op.Equals( "%" ) )
					return( toValue(arg1) % toValue(arg2)); 
				else if( op.Equals( "==" ) )
					return( toValue(arg1) == toValue(arg2) ? 1.0 : 0.0 );
				else if( op.Equals( "!=" ) )
					return( toValue(arg1) != toValue(arg2) ? 1.0 : 0.0  );
				else if( op.Equals( "<" ) )
					return( toValue(arg1) < toValue(arg2) ? 1.0 : 0.0 );
				else if( op.Equals( ">" ) )
					return( toValue(arg1) > toValue(arg2) ? 1.0 : 0.0 );
				else if( op.Equals( "&&" ) )
					return( ( toValue(arg1) == 1.0 ) && ( toValue(arg2) == 1.0 ) ? 1.0 : 0.0 );
				else if( op.Equals( "||" ) )
					return( ( toValue(arg1) == 1.0 ) || ( toValue(arg2) == 1.0 ) ? 1.0 : 0.0 );
				else if( op.Equals( ">=" ) )
					return( toValue(arg1) >= toValue(arg2) ? 1.0 : 0.0 );
				else if( op.Equals( "<=" ) )
					return( toValue(arg1) <= toValue(arg2) ? 1.0 : 0.0 );
			
	   
			}
			else
			{
				if( op.Equals( "sqrt" ) )
					return( Math.Sqrt(toValue(arg1)));
				else if( op.Equals( "sin" ) )
					return( Math.Sin(toValue(arg1)));
				else if( op.Equals( "cos" ) )
					return( Math.Cos(toValue(arg1)));
				else if( op.Equals( "tan" ) )
					return( Math.Tan(toValue(arg1)));
				else if( op.Equals( "asin" ) )
					return( Math.Asin(toValue(arg1)));
				else if( op.Equals( "acos" ) )
					return( Math.Acos(toValue(arg1)));
				else if( op.Equals( "atan" ) )
					return( Math.Atan(toValue(arg1)));
				else if( op.Equals( "ln" ) )
					return( Math.Log(toValue(arg1)));
				else if( op.Equals( "exp" ) )     
					return( Math.Exp(toValue(arg1)));
				else if( op.Equals( "cotan" ) )
					return(1 / Math.Tan(toValue(arg1)));
				else if( op.Equals( "acotan" ) )
					return( Math.PI / 2 - Math.Atan(toValue(arg1)));
				else if( op.Equals( "ceil" ) )
					return((double)Math.Ceiling(toValue(arg1)));
				else if( op.Equals( "round" ) )
					return((double)Math.Round(toValue(arg1)));
				else if( op.Equals( "floor" ) )
					return((double)Math.Floor(toValue(arg1)));
				else if( op.Equals( "fac" ) )
					return(fac(toValue(arg1)));
				else if( op.Equals( "abs" ) )
					return( Math.Abs(toValue(arg1)));
				else if( op.Equals( "fpart" ) )
					return( fpart(toValue(arg1)) );
				else if( op.Equals( "sfac" ) )
					return( sfac(toValue(arg1)));
				else if( op.Equals( "sinh" ) )
				{
					val = toValue(arg1);
					return( ( Math.Exp(val) - ( 1 / Math.Exp(val))) / 2);
				}
				else if( op.Equals( "cosh" ) )
				{
					val = toValue(arg1);
					return( ( Math.Exp(val) + ( 1 / Math.Exp(val))) / 2);
				}
				else if( op.Equals( "tanh" ) )
				{
					val = toValue(arg1);
					return( ( ( Math.Exp(val) - ( 1 / Math.Exp(val))) / 2) / ((Math.Exp(val) + ( 1 / Math.Exp(val))) / 2) ); 
				}
				else if( op.Equals( "!" ) )
				{
					return( ( ! ( toValue(arg1) == 1.0 ) ) ? 1.0 : 0.0 );
				}
			}
		  
			throw new System.Exception( "Unknown operator" );
		   
		}



		/// <summary>
		/// Retrieves a value stored in the Hashtable containing all variable = value pairs.
		/// </summary>
		/// <remarks>
		/// The hashtable used in this method is set by the Parse( String, Hashtable ) method so this method retrives
		/// values inserted by the user of this class. Please note that no processing has been made
		/// on these values, they may have incorrect syntax or casing.
		/// </remarks>
		/// <param name="key">the name of the variable we want the value for</param>
		/// <returns>the value stored in the Hashtable or null if none.</returns>
		private double GetVariable( String key ) 
		{
			string[] strPrmSplit = key.Trim('[',']').Split(':');

			Cell cell = view.GetCell(strPrmSplit[0], strPrmSplit[1]);
			if(cell!=null)
				return cell.Value;

			return 0.0;
		}



		/// <summary>
		/// Evaluates the infix expression using the values in the Hashtable.
		/// </summary>
		/// <remarks>
		/// This is the only publicly available method of the class, it is the entry point into for the user 
		/// into the parser.
		///
		/// Example usage:
		/// 
		/// using info.lundin.Math;
		/// using System;
		/// using System.Collections;
		///
		/// public class Test 
		/// {
		/// 	public static void Main( String[] args )
		/// 	{
		/// 		ExpressionParser parser = new ExpressionParser();
		/// 		Hashtable h = new Hashtable();
		///
		/// 		h.Add( "x", 1.ToString() );
		/// 		h.Add( "y", 2.ToString() );
		///
		///
		/// 		double result = parser.Parse( "xcos(y)", h );
		/// 		Console.WriteLine( “Result: {0}”, result );
		/// 	}
		/// }
		///
		/// </remarks>
		/// <param name="exp">the infix string expression to parse and evaluate.</param>
		/// <returns>a double value</returns>
		public double Parse(String exp, SpreadSheetView view) 
		{
			double ans = 0D;
			String tmp;
			Node tree;

			if( exp == null || exp.Equals("") )
			{
				throw new System.Exception("First argument to method eval is null or empty string");
			}
			else if( view == null )
			{
				throw new ArgumentNullException("View");  
			}
		 
			this.view	= view;

			tmp = skipSpaces( exp );
			//this.sb_init = tmp.Length;

			try
			{
		    
				if( trees.ContainsKey( tmp ) )
				{
					ans = toValue( (Node)trees[ tmp ] );   		
				}
				else
				{
					Syntax( tmp );
					
					//tree = parse( putMult( parseE( tmp ) ) ); 
					tree = parse(  parseE( tmp )  );
			
					ans = toValue( tree );

					trees.Add( tmp , tree );  
				}
	
				return ans;
		  
			}
			catch( Exception e )
			{ 
				throw new System.Exception( e.Message );
			}
		}

		
	} // End class ExpressionParse


	/// <summary>
	/// Class Node, represents a Node in a tree data structure representation
	/// of a mathematical expression.
	/// </summary>
	public class Node
	{

		/// <summary>Represents the type variable</summary>
		public static int TYPE_VARIABLE		= 1;

		/// <summary>Represents the type constant ( numeric value )</summary>
		public static int TYPE_CONSTANT		= 2;

		/// <summary>Represents the type expression</summary>
		public static int TYPE_EXPRESSION	= 3;

		/// <summary>Reserved</summary>
		public static int TYPE_END		= 4;

		/// <summary>Used as initial value</summary>
		public static int TYPE_UNDEFINED	= -1;

		private String _operator		= ""; 
		private Node _arg1		= null;
		private Node _arg2		= null;
		private int args		= 0;
		private int type		= TYPE_UNDEFINED;
		private double value		= Double.NaN;
		private String variable		= "";
		
		/// <summary>
		/// Creates a Node containing the specified Operator and arguments.
		/// This will automatically mark this Node as a TYPE_EXPRESSION
		/// </summary>
		/// <param name="_operator">the string representing an operator</param>
		/// <param name="_arg1">the first argument to the specified operator</param>
		/// <param name="_arg2">the second argument to the specified operator</param>
		public Node( String _operator, Node _arg1, Node _arg2 )
		{
			this._arg1	= _arg1;
			this._arg2	= _arg2;
			this._operator	= _operator;
			this.args	= 2;
			this.type	= TYPE_EXPRESSION;
		}

		/// <summary>
		/// Creates a Node containing the specified Operator and argument.
		/// This will automatically mark this Node as a TYPE_EXPRESSION
		/// </summary>
		/// <param name="_operator">the string representing an operator</param>
		/// <param name="_arg1">the argument to the specified operator</param>
		public Node( String _operator, Node _arg1 )
		{
			this._arg1	= _arg1;
			this._operator	= _operator;	
			this.args	= 1;
			this.type	= TYPE_EXPRESSION;
		}

		/// <summary>
		/// Creates a Node containing the specified variable.
		/// This will automatically mark this Node as a TYPE_VARIABLE
		/// </summary>
		/// <param name="variable">the string representing a variable</param>
		public Node( String variable )
		{
			this.variable	= variable;
			this.type	= TYPE_VARIABLE;
		}

		/// <summary>
		/// Creates a Node containing the specified value.
		/// This will automatically mark this Node as a TYPE_CONSTANT
		/// </summary>
		/// <param name="value">the value for this Node</param>
		public Node( double value )
		{
			this.value	= value;
			this.type	= TYPE_CONSTANT;
		}

		/// <summary>
		/// Returns the String operator of this Node 
		/// </summary>
		public String getOperator()
		{
			return( this._operator );	
		}

		/// <summary>
		/// Returns the value of this Node 
		/// </summary>
		public double getValue()
		{
			return( this.value );	
		}

		/// <summary>
		/// Returns the String variable of this Node 
		/// </summary>
		public String getVariable()
		{
			return( this.variable );	
		}
		
		/// <summary>
		/// Returns the number of arguments this Node has
		/// </summary>
		public int arguments()
		{
			return( this.args );
		}

		/// <summary>
		/// Returns the type of this Node
		/// </summary>
		/// <remarks>
		/// The type can be:
		///	Node.TYPE_VARIABLE
		///	Node.TYPE_CONSTANT
		///	Node.TYPE_EXPRESSION
		/// </remarks>
	
		public int getType()
		{
			return( this.type );
		}
		
		/// <summary>
		/// Returns the first argument of this Node
		/// </summary>
		public Node arg1()
		{
			return( this._arg1 );	
		}
		
		/// <summary>
		/// Returns the second argument of this Node
		/// </summary>
		public Node arg2()
		{
			return( this._arg2 );	
		}

	} // End class Node


	/// <summary>
	/// Class Operator, represents an Operator by holding information about it's symbol
	/// the number of arguments it takes and the operator precedence.
	/// </summary>
	public class Operator
	{

		private String op = ""; // the string operator 
		private int args  = 0; // the number of arguments this operator takes
		private int prec  = System.Int32.MaxValue; // the precedence this operator has

		/// <summary>
		/// Creates an Operator with the specified String name, arguments and precedence
		/// </summary>
		public Operator( String _operator, int arguments, int precedence )
		{
			this.op	  = _operator;
			this.args = arguments;
			this.prec = precedence;
		}

		/// <summary>
		/// Returns the precedence for this Operator.
		/// </summary>
		public int precedence()
		{
			return( this.prec );
		}

		/// <summary>
		/// Returns the String name of this Operator.
		/// </summary>
		public String getOperator()
		{
			return( this.op );
		}

		/// <summary>
		/// Returns the number of arguments this Operator can take.
		/// </summary>
		public int arguments()
		{
			return( this.args );
		}

	} // End class Operator


} // End namespace info.lundin.math