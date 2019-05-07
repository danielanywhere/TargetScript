//	Program.cs
//	Main application service module.
//
//	Copyright (c). 2018, 2019 Daniel Patterson, MCSD (danielanywhere)
//	Released for public access under the MIT License.
//	http://www.opensource.org/licenses/mit-license.php
using System;
using System.IO;
using System.Text;

namespace TargetScript
{
	//*-------------------------------------------------------------------------*
	//*	Program																																	*
	//*-------------------------------------------------------------------------*
	/// <summary>
	/// Main instance of the application.
	/// </summary>
	public class Program
	{
		//*************************************************************************
		//*	Private																																*
		//*************************************************************************
		//*************************************************************************
		//*	Protected																															*
		//*************************************************************************
		//*************************************************************************
		//*	Public																																*
		//*************************************************************************
		//*-----------------------------------------------------------------------*
		//*	_Main																																	*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Command-line startup of the application.
		/// </summary>
		/// <param name="args">
		/// Array of command-line arguments.
		/// </param>
		public static void Main(string[] args)
		{
			bool bError = false;		//	Flag - Error.
			bool bShow = false;			//	Flag - Explicit Show.
			FileInfo ifile = null;  //	Working File Tester.
			int iValue = 0;
			string key = "";				//	Current Parameter Key.
			string lowerArg = "";   //	Current Lowercase Argument.
															//	User Message.
			StringBuilder message = new StringBuilder();
			Program prg = new Program();  //	Initialized instance.

			Console.WriteLine("TargetScript.exe");
			foreach(string arg in args)
			{
				lowerArg = arg.ToLower();
				key = "/?";
				if(lowerArg == key)
				{
					bShow = true;
					continue;
				}
				key = "/createtemplate:";
				if(lowerArg.StartsWith(key))
				{
					prg.mMode = ProgramModeTypeEnum.ConvertToTemplate;
					prg.mProjectFilename = arg.Substring(key.Length);
					ifile = new FileInfo(prg.mProjectFilename);
					continue;
				}
				key = "/mode:";
				if(lowerArg.StartsWith(key))
				{
					Enum.TryParse<ProgramModeTypeEnum>(
						arg.Substring(key.Length), true, out prg.mMode);
				}
				key = "/output:";
				if(lowerArg.StartsWith(key))
				{
					prg.mOutputFilename = arg.Substring(key.Length);
					continue;
				}
				key = "/project:";
				if(lowerArg.StartsWith(key))
				{
					prg.mProjectFilename = arg.Substring(key.Length);
					continue;
				}
				key = "/projectpath:";
				if(lowerArg.StartsWith(key))
				{
					prg.mProjectPath = arg.Substring(key.Length);
					continue;
				}
				key = "/tabchar:";
				if(lowerArg.StartsWith(key))
				{
					//	't' or 's'.
					switch(arg.Substring(key.Length).ToLower())
					{
						case "s":   //	Space.
							prg.mTabCharacter = ' ';
							break;
						case "t":   //	Tab.
							prg.mTabCharacter = '\t';
							break;
					}
				}
				key = "/tabcount:";
				if(lowerArg.StartsWith(key))
				{
					int.TryParse(arg.Substring(key.Length), out iValue);
					prg.mTabCount = iValue;
				}
				key = "/wait";
				if(lowerArg.StartsWith(key))
				{
					prg.mWaitAfterEnd = true;
					continue;
				}
			}
			if(!prg.mShowSyntax && !bError && !bShow)
			{
				if(message.Length > 0)
				{
					Console.WriteLine(message);
				}
				prg.Run();
			}
			else
			{
				//	Display Syntax.
				Console.WriteLine(message.ToString() + "\r\n" + ResourceMain.Syntax);
			}
			if(prg.mWaitAfterEnd)
			{
				Console.WriteLine("Press [Enter] to exit...");
				Console.ReadLine();
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Components																														*
		//*-----------------------------------------------------------------------*
		private ComponentCollection mComponents = new ComponentCollection();
		/// <summary>
		/// Get a reference to the collection of component definitions.
		/// </summary>
		public ComponentCollection Components
		{
			get { return mComponents; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Configurations																												*
		//*-----------------------------------------------------------------------*
		private ConfigurationCollection mConfigurations =
			new ConfigurationCollection();
		/// <summary>
		/// Get a reference to the collection of configuration sets.
		/// </summary>
		public ConfigurationCollection Configurations
		{
			get { return mConfigurations; }
			set { mConfigurations = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Mode																																	*
		//*-----------------------------------------------------------------------*
		private ProgramModeTypeEnum mMode = ProgramModeTypeEnum.RenderProject;
		/// <summary>
		/// Get/Set the mode of operation.
		/// </summary>
		public ProgramModeTypeEnum Mode
		{
			get { return mMode; }
			set { mMode = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	OutputFilename																												*
		//*-----------------------------------------------------------------------*
		private string mOutputFilename = "";
		/// <summary>
		/// Get/Set the name of the explicitly specified output filename.
		/// </summary>
		public string OutputFilename
		{
			get { return mOutputFilename; }
			set { mOutputFilename = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProjectFilename																												*
		//*-----------------------------------------------------------------------*
		private string mProjectFilename = "";
		/// <summary>
		/// Get/Set the name of the project file to open.
		/// </summary>
		public string ProjectFilename
		{
			get { return mProjectFilename; }
			set { mProjectFilename = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ProjectPath																														*
		//*-----------------------------------------------------------------------*
		private string mProjectPath = "";
		/// <summary>
		/// Get/Set the Path of the Project.
		/// </summary>
		public string ProjectPath
		{
			get { return mProjectPath; }
			set { mProjectPath = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Run																																		*
		//*-----------------------------------------------------------------------*
		/// <summary>
		/// Run the configured application.
		/// </summary>
		public void Run()
		{
			ActionNodeTree tree = new ActionNodeTree();
			string content = "";
			DirectoryInfo idir = null;
			FileInfo ifile = new FileInfo(mProjectFilename);
			FileInfo ofile = null;

			if(mProjectPath?.Length > 0)
			{
				//	If a project path was specified, then set it.
				idir = new DirectoryInfo(mProjectPath);
				Directory.SetCurrentDirectory(mProjectPath);
			}
			else if(ifile.DirectoryName?.Length > 0)
			{
				//	If the project file used a directory name, then that
				//	will also be our project folder.
				if(ifile.DirectoryName.IndexOf(":") > -1 ||
					ifile.DirectoryName.IndexOf("\\\\") > -1)
				{
					//	Absolute path in project filename overrides project path
					//	name.
					Directory.SetCurrentDirectory(ifile.DirectoryName);
					mProjectPath = ifile.DirectoryName;
				}
				else
				{
					//	Project filename path is relative.
					mProjectPath = Path.Combine(mProjectPath, ifile.DirectoryName);
					Directory.SetCurrentDirectory(mProjectPath);
				}
				mProjectFilename = ifile.Name;
			}
			else
			{
				//	Use the current directory as the project path.
				mProjectPath = Directory.GetCurrentDirectory();
			}

			ifile = new FileInfo(Path.Combine(mProjectPath, mProjectFilename));
			if(mOutputFilename?.Length > 0)
			{
				if(Tools.IsRelative(mOutputFilename))
				{
					ofile = new FileInfo(Path.Combine(mProjectPath, mOutputFilename));
				}
				else
				{
					ofile = new FileInfo(mOutputFilename);
				}
			}

			if(ifile.Exists)
			{
				//	Read the project file.
				content = File.ReadAllText(ifile.FullName);
				switch(mMode)
				{
					case ProgramModeTypeEnum.ConvertToTemplate:
						Console.WriteLine("Mode: Convert source to template...");
						content =
							JsonTemplateItem.MakeTemplate(content,
							mTabCharacter, mTabCount);
						if(ofile != null)
						{
							Directory.CreateDirectory(ofile.DirectoryName);
							File.WriteAllText(ofile.FullName, content);
							Console.WriteLine(
								string.Format("Template file written to {0}...",
								ofile.Name));
						}
						break;
					case ProgramModeTypeEnum.InventoryProject:
						Console.WriteLine("Mode: Create inventory report from project...");
						JsonTemplateCollection.Parse(content, mProjectPath,
							mConfigurations, mComponents, mTemplates);
						tree.Configurations = mConfigurations;
						tree.Components = mComponents;
						tree.ProjectPath = mProjectPath;
						tree.RenderedContent.Clear();
						foreach(TemplateItem template in mTemplates)
						{
							tree.InventoryTemplate(template);
						}
						if(tree.SaveFile(ofile))
						{
							Console.WriteLine(
								string.Format(
								"File {0} saved...", ofile.Name));
						}
						break;
					case ProgramModeTypeEnum.ListNodes:
						Console.WriteLine("Mode: List nodes in project...");
						JsonTemplateCollection.Parse(content, mProjectPath,
							mConfigurations, mComponents, mTemplates);
						tree.Configurations = mConfigurations;
						tree.Components = mComponents;
						tree.ProjectPath = mProjectPath;
						tree.RenderedContent.Clear();
						foreach(TemplateItem template in mTemplates)
						{
							tree.ListTemplate(template);
						}
						if(tree.SaveFile(ofile))
						{
							Console.WriteLine(
								string.Format(
								"File {0} saved...", ofile.Name));
						}
						break;
					case ProgramModeTypeEnum.RenderProject:
						Console.WriteLine("Mode: Render documents from project...");
						JsonTemplateCollection.Parse(content, mProjectPath,
							mConfigurations, mComponents, mTemplates);
						tree.Configurations = mConfigurations;
						tree.Components = mComponents;
						tree.ProjectPath = mProjectPath;
						foreach(TemplateItem template in mTemplates)
						{
							Console.WriteLine(
								string.Format("Rendering template: {0}",
								template.Name));
							if(tree.RenderTemplate(template))
							{
								if(tree.RenderedContent.IsDirty)
								{
									//	Save the file if possible.
									if(tree.SaveFile())
									{
										Console.WriteLine(
											string.Format(
											"File {0} saved...",
											mConfigurations.GetValue(
												template.Name + ".OutputFilename")));
									}
								}
							}
						}
						break;
				}
			}
			else
			{
				Console.WriteLine("Error: Project File [" + ifile.FullName +
					"] does not exist...");
			}
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	ShowSyntax																														*
		//*-----------------------------------------------------------------------*
		private bool mShowSyntax = false;
		/// <summary>
		/// Get/Set a value indicating whether to display the usage syntax.
		/// </summary>
		public bool ShowSyntax
		{
			get { return mShowSyntax; }
			set { mShowSyntax = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TabCharacter																													*
		//*-----------------------------------------------------------------------*
		private char mTabCharacter = '\t';
		/// <summary>
		/// Get/Set the default tab character for use in this session.
		/// </summary>
		public char TabCharacter
		{
			get { return mTabCharacter; }
			set { mTabCharacter = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	TabCount																															*
		//*-----------------------------------------------------------------------*
		private int mTabCount = 1;
		/// <summary>
		/// Get/Set the number of characters to insert for each tab space.
		/// </summary>
		public int TabCount
		{
			get { return mTabCount; }
			set { mTabCount = value; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	Templates																															*
		//*-----------------------------------------------------------------------*
		private TemplateCollection mTemplates = new TemplateCollection();
		/// <summary>
		/// Get a reference to the collection of data templates.
		/// </summary>
		public TemplateCollection Templates
		{
			get { return mTemplates; }
		}
		//*-----------------------------------------------------------------------*

		//*-----------------------------------------------------------------------*
		//*	WaitAfterEnd																													*
		//*-----------------------------------------------------------------------*
		private bool mWaitAfterEnd = false;
		/// <summary>
		/// Get/Set a value indicating whether to wait for a keypress after the
		/// end of the application.
		/// </summary>
		public bool WaitAfterEnd
		{
			get { return mWaitAfterEnd; }
			set { mWaitAfterEnd = value; }
		}
		//*-----------------------------------------------------------------------*

	}
	//*-------------------------------------------------------------------------*
}
