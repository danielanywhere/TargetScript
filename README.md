# TargetScript
A code and script automation system by Daniel Patterson. <danielanywhere@outlook.com>
<hr>

# Introduction
TargetScript is all about the concept of code and script automation. Instead of writing code straight out, this system allows you to describe the basics of what the code is, then lets the code write and maintain itself from closely organized modular chunks based upon those descriptions.

You can speed up your code-writing tasks to finish your work several times faster than your peers by switching to this kind of declarative approach. In larger projects where several components are involved, you might even experience an exponential increase in output.

Using TargetScript, you can work smarter and faster instead of working harder, while simultaneously producing more consistent and readable work free of random errors.

## What is TargetScript?
In as few words as possible, TargetScript is a processor for rendering highly abstracted inline templating statements.

From a more relaxed user perspective, TargetScript like a set of JSON data files that have been organized in a certain way as to be readable by the TargetScript command-line utility. Most of this data defines the nature of the system through configuration settings and metadata, while a smaller part of it serves the purpose of defining specific behavior through interchangeable templates.

When a set of data files are rendered by running TargetScript on them, finished documents are created in the process that can be used for any number of purposes. With version 1.0 of TargetScript, I show you examples of how the same metadata files are used several times to produce all of the source code files I need for constructing and compiling the entire _BankCore_ project listed elsewhere on my profile.

The TargetScript system utilizes the concept of replaceable variables to create logical placeholders for just about anything that might not be known at design time; might be replaced without prior notice; might or might not be included in the output, depending on one or more conditions; or might be repeated as a pattern a number of times.

### {CurlyBraceEncapsulation}
When coming from local transient settings or from the master configuration table in the project, variables are referenced within tight curly brace configuration.

For example, when describing the path to your network database, you might use the configuration variable named {InternalDatabaseIP}, as in the following text.
<blockquote><pre>"Next, verify you can ping the database
server at {InternalDatabaseIP}."</pre></blockquote>

As mentioned above, the configuration tables for the project contain permanent values for use by the entire project, but template nodes can have a number of similarly formed transient values as well. Node-based configuration values are handled according to their relative hierarchy to one another, and in general, the closest named value to the item being resolved will be the one used in the present resolution.

### [SquareBraceEncapsulation]
Values and object definitions that are used in a list-like, repeatable, or loop-like fashion are located in the metadata tables. These tables, also referred to as _components_ and _component pages_, are able to organize data for a number of separate objects that share similar attributes, in the same sense that there are different models of cars, yet they all share similar attribute identifications like paint color; engine size; wheel count; tire size; tire tread pattern; and number of doors, etc.

It is exactly through this type of context-specific attribute sharing that you reduce repetitive tasks in media design to the bare minimum.

For example, when creating the salutation on a form letter to the customer, you might use the variable CustomerName as follows.

<blockquote><pre>   Dear [CustomerName],
It has come to our attention that you are
the best customer ever.
</pre></blockquote>

More specifically, it is not only possible but easy to extend your metadata definition to include a new field that only needs to be filled in before providing value to your template. In the above scenario, let's say we wanted to add the honorific to the customer form letter template.

There are only three steps to this improvement.

1. Add the definition to the metadata customer components JSON, as in:<br />
"Honorific": "Mr. "<br /><br />
2. Add the implementation to the template above:
<blockquote><pre>   Dear [Honorific][CustomerName],
It has come to our attention that you are
the best customer ever.
</pre></blockquote>

3. Rerun the TargetScript application on the project to reproduce all of the resulting documents.

### General Rules for Tightly Braced Values
In general, there are only a few rules for square and curly tightly braced variable names.

 - All configuration values in the configuration table are created by you.
 - Most transient configuration values in nodes are created by implication, and some of them are named by you.
 - All metadata values for components, except 'Name', are created by you.
 - Every variable name must touch both sides of the enclosure. For example, { ThisVariable } is not a legal variable.
 - The variable name must not contain spaces.
 - Every variable name must start with a letter.
 - Other than in the first position, variable names can contain letters, numbers, hyphens, and underscores.

### {Commands(x)}
TargetScript has support for a number of inline commands that help with branching and flow.

#### Example Command - {LoopStart(_Name_:_Value_, ...)} / {LoopEnd(_Name_:_Value_)}
The loop start command is placed prior to an area of the template that will be repeated a number of times, using the component set as the group of objects to be considered for output to the document.

For example, let's say the template contains the following pattern:
<blockquote><pre>&lt;ul&gt
{LoopStart(name:bullet;level:component)}
 &lt;li&gt;{ObjectName}&lt;/li&gt;
{LoopEnd(name:bullet)}
&lt;/ul&gt;
</pre></blockquote>
In the above example, the node-based variable {ObjectName} represents the name of the currently selected transient component, which in this case has the same effect as using [Name].

Also notice that in this example, the user-defined name "bullet" was used explicitly for terminating the loop. In TargetScript, if you omit the name parameter from LoopEnd, rendering will return automatically to the closest loop in the hierarchy. However, unlike conventional coding systems, you are also allowed to pop out of multiple levels of a loop with one LoopEnd statement. For example, in the case <code>{LoopStart(name:one)}1 {LoopStart(name:two)}2 {LoopStart(name:three)}3 {LoopEnd(name:one)}</code>, all three loops are terminated at the outermost cycle without any damage to the repeated loop behavior.

In addition to specifying the **name** and **level** of a loop, you can also specify an **expression** that will only render the items whose statements match values found in the configuration table, passing node, or currently selected component or metadata field.

Continuing on with the previous example, if our metadata component names are loaded with the random words "Verify", "Tiger", "Fresh", "Round", and "Wool", then the above example is transformed to the pattern below:

<blockquote><pre>&lt;ul&gt;
 &lt;li&gt;Verify&lt;/li&gt;
 &lt;li&gt;Tiger&lt;/li&gt;
 &lt;li&gt;Fresh&lt;/li&gt;
 &lt;li&gt;Round&lt;/li&gt;
 &lt;li&gt;Wool&lt;/li&gt;
&lt;/ul&gt;
</pre></blockquote>

During the rendering process, templated function names are also removed from the output content like their variable counterparts, leaving only the resolved, ready to consume content.

Similarly to the construct illustrated here, TargetScript provides support for a number of block operations, commands, and flow control conditions, all of which are described in the Wiki article about using commands.


## Added Benefits
Adopting this approach to coding or object design can not only greatly enhance your abilities to near superhuman levels, but can provide the following additional benefits.

 - **Accessibility**. Thanks to the modular arrangement of the input data, less experienced workers can be assigned to product construction tasks, and be working on limited complexity artifacts at the same time as more experienced workers are handling changes to other, more difficult areas of the same project. Equally important, a new worker can become functional within the system in a very short period of time after first seeing it.<br /><br />
 - **Adaptability**. TargetScript can output any language for which a template has been attached on the current project. This includes both languages that went out of style 50 years ago and languages that are not yet invented. Whether you are publishing a Mad-Lib newsletter, handling job estimation for a construction company based upon a long set of parameters, writing long PowerShell scripts for installing your application on a PC, or creating an enterprise-level business database system and compiling for several different software platforms and languages at once, TargetScript is generic enough to produce surprisingly quick, reliable, and repeatable results.<br /><br />
 - **Configurability**. Changing a project often only involves changing or adding a base setting in the metadata, making a tiny adjustment to one or more templates, then re-rendering the entire project to produce the finished models or source code.<br /><br />
 - **Maintainability**. Repairing one bug in the project will result in the immediate repair of all related bugs.<br /><br />
 - **Expeditability**. What used to take a team several months to complete can now be accomplished by one or as few as three people in a number of days.<br /><br />
 - **Modularity**. Parts of your current and other projects can be switched in and out within a couple of minutes, completely changing the capabilities of the rendered product. In the case where a development team can maintain a robust library of functionality for background purposes, less expert information workers can pick and place from that library to produce functional and reliable documents, models, or software, as the case may be.<br /><br />
 - **Readability**. Many people find that reading through a list of descriptive or declarative information is several times easier to absorb and understand than an equal amount of imperative text. This can be illustrated as the comparison between a the individual values on a spreadsheet and a C language computer source code file, respectively.<br /><br />
 - **Reliability**. Once a logic function has been tested to work, it can be repeatedly reused in an unlimited number of future contexts where only its physical details (the equivalent of nouns) are switched in and out on a use by use basis. Since the behavior of the function itself never changes, it is known to be working before the output is ever compiled. In a separate but important argument, the number of errors related to typed pattern repetition is also reduced to near or exactly zero. In the best scenario, all repeated patterns are handled by TargetScript.<br /><br />
 - **Reusability**. All of the parts of every project can be independently filed in an organized and functional format that promotes the growth, over time, of a truly manageable library. Any of the parts in the library can be used for previously unexpected purposes, or can be researched and selected as candidates for new projects. If one function is reused in another project, there is not necessarily any dependency present requiring another similar function to also be used in that case.<br /><br />
 - **Verifiability**. Since the base intelligence of the writing is available as cellular, partitioned data on one side and templated snippet prototypes on the other, the work is primely organized for a wide delegation of duty between data or product-centric engineers on one side and logic-centric engineers on the other. Peer verification at this level of separation is a breeze, because the entire result is either correct or it is not. This effect allows for a very large project to be released in a surprisingly short period of time.<br /><br />
