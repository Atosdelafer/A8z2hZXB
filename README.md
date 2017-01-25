# Prolog Embedding in C&#35;

This repository contains the results of our attempt to embed Prolog in C#, a general-purpose logic programming language.
It is considered as one of the first and most popular logic programming language available.
Originally Prolog was intended as a way to process natural language, but it has also been used for theorem proving and expert systems.

The language is suited for situations in which rule-based logical queries are used, for example databases or voice processing systems.
It originated from first-order logic and is, unlike most programming languages, declarative.
This means that the flow is expressed in terms of relation represented as fact and rules.
By running a query over these relations, a computation can be done.

## More details
In the <a href="https://github.com/winandr/A8z2hZXB/tree/master/reports" target="_blank">this</a> directory more details about this project can be found in several reports

##Directory structure

In the following, we detail per code file what the points of interest are an give an explanation of their content. 

## Program.cs

###Main

In the main function we first call the FileReader method, which meakes sure the file containing the knowledge database is read in.
Next the lines scanned are checked via the methods in ClauseValidator.cs on wheter they are valid Prolog knowledge database entries and they are put into the specially designed data structure of a TermTree, explained in the TermTreeIndex.cs file.
Clauses are stored in a seperate structure with their tails, which is needed for proper unification later on.
Finally we arrive at the most important part of the method, where we filter output of a query to be displayed in a proper manner, but mostly where the program keeps on accepting input and conducting Inference. This is done specifically by the call to the inference method as inference(inputline).

###Inference
The inference method handles all inference done on the database. First the TermTree structure is consulted via the getMatchingTrees2 method. This returns any matches of the specific input we used to query with, in a complicated yet usedful structure. In a scan over this structure, we distinguish betweern queries where there are vaiables to match and queries where no variables are present. In the first case the headRecursionVar is called, in the second case we note down the line numbers matching to the query and later on use these in a call to headRecursion.

###HeadRecursionVar

First for the query, we check for each line that matched if it was a clause or not. In case it was not a clause, we simply match the variables of the query to the corresponding terms in the line and return them. If the query consisted of party unbounded variables and partly already bounded variables, we add True, which is just an intermediate step to identify the variable is already matched.

If a line is a clause, things are a bit more complicated, however. First, we identify which variables are bound to the internal variables of the database. That is, if we have "parent(X,Y):- something" in the database, and we query parent(O,P) instead, these variable bindings are stored. Next the method Testmethod is called, which is designed to seperate the items in the tail of a clause into terms which are in the correct format to be entered into the inference function recursively later on. Then we save the arity of each term in the claus tail, which will be needed later on when looking at all the possible results a query can return.

Next we go into recursion, calling inference() on the seperate parts of the tail. The resulting options are stored. At this point an imporant distinction is made again, if there was only one term in the tail, we have got the result of the inference on this term already and we can return the variable bindings we found as the result. If there are multiple parts in the tail however, things get considerably more complicated. In the latter case, all combinations of variable bindings of the seperate terms in the tail have to be checked for consistency. As there is no maximum on the number of terms a tail might contain, an eleborate enumeration method has been devised, depending on the arity of the different terms. Any combination of terms found to have a consistent variable assignment is stored and later on returned to inference().

###HeadRecursion

Should the query not contain any variables, or should all variables be bound, inference() calls not the headRecursionVar method but the headRecursion method instead. This method is quite similar to HeadRecursionVar, but is simpler due to the lack of variables which are not yet bound. This makes comparison of terms a lot more straightforward and the use of a function like Testmethod to prepare the terms in the clausetail to be used as a query is not neccesary. The functionality of the method can be understood in a straightforward manner by comparing to the description of headRecursionVar.
