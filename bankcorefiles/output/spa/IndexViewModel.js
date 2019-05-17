/*****************************************************************************/
/* Global Functions and Values                                               */
/*****************************************************************************/
/*---------------------------------------------------------------------------*/
/* booleanToNumber                                                           */
/*---------------------------------------------------------------------------*/
/**
	* Return a 1 or 0 number value corresponding to the caller's input.
	* @param {boolean} value Boolean value to inspect.
	* @returns {number} 1 or 0 representation of the caller's value.
	*/
function booleanToNumber(value)
{
	return (value ? 1 : 0);
}
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* getFormattedDate                                                          */
/*---------------------------------------------------------------------------*/
/**
	* Return the user-readable version of a date.
	* @param {object} date The date to convert to printable.
	* @returns {string} The caller's date, formatted for MM/DD/YYYY.
	*/
function getFormattedDate(date)
{
 var day = date.getDate().toString();
 var month = (1 + date.getMonth()).toString();
	var result = "";
 var year = date.getFullYear();

 month = month.length > 1 ? month : '0' + month;
 day = day.length > 1 ? day : '0' + day;

	result = month + '/' + day + '/' + year;
	if(result == "12/31/1969")
	{
		result = "";
	}
	return result;
}
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* loadData                                                                  */
/*---------------------------------------------------------------------------*/
/**
	* Load the server data and populate the local tables.
	*/
function loadData()
{
	var data = null;
	var reply = "";

	$(".jsGrid").jsGrid("_showLoading");
	$.ajax({
		type: "GET",
		url: "/indexdata",
		success: loadDataSuccess,
		complete: loadDataComplete
	});
}
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* loadDataComplete                                                          */
/*---------------------------------------------------------------------------*/
/**
	* Called when the data load operation has completed.
	* @param {object} jqXHR Equivalent of the XMLHttpRequest object.
	* @param {string} status Status message of the operation.
	*/
function loadDataComplete(jqXHR, status)
{
	$(".jsGrid").jsGrid("_hideLoading");
}
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* loadDataSuccess                                                           */
/*---------------------------------------------------------------------------*/
/**
	* Called when the data load operation has successfully received data from the
	* server.
	* @param {object} result Data received from the server. If everything went as
	* expected, the format will already be JSON.
	*/
function loadDataSuccess(result)
{
	var rCount = 0;
	var rIndex = 0;
	var target = null;
	var tCount = result.length;
	var tIndex = 0;
	var tItem = null;
	console.log("loadData: received " + tCount + " tables...");
	for(tIndex = 0; tIndex < tCount; tIndex ++)
	{
		tItem = result[tIndex];
		console.log("Set " + tItem.Name + " data / record count: " + tItem.Table.length);
		target = null;
		switch(tItem.Name)
		{
			case "Accounts":
				target = accountsViewModel.accounts;
				break;
			case "Branches":
				target = branchesViewModel.branches;
				break;
			case "Customers":
				target = customersViewModel.customers;
				break;
			case "Employees":
				target = employeesViewModel.employees;
				break;
		}
		if(target)
		{
			target.length = 0;
			rCount = tItem.Table.length;
			for(rIndex = 0; rIndex < rCount; rIndex ++)
			{
				// console.log(" adding record " + rIndex);
				target.push(tItem.Table[rIndex]);
			}
			$("#grd" + tItem.Name).jsGrid("loadData");
			$("#grd" + tItem.Name).jsGrid("refresh");
		}
	}
}
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* toBoolean                                                                 */
/*---------------------------------------------------------------------------*/
/**
	* Return a value indicating whether the caller's parameter evaluates to
	* true or false.
	* @param {object} value Value to inspect for boolean outcome.
	* @returns {boolean} Boolean representation of the caller's value.
	*/
function toBoolean(value)
{
	var result = false;
	if(typeof(value) === "string")
	{
		value = value.trim().toLocaleLowerCase();
	}
	switch(value)
	{
		case true:
		case "true":
		case 1:
		case "1":
		case "on":
		case "yes":
			result = true;
			break;
	}
	return result;
}
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* toNumber                                                                  */
/*---------------------------------------------------------------------------*/
/**
	* Return a numeric representation of the caller's value.
	* @param {string} value Value to convert to number.
	* @returns {number} Numeric representation of the caller's value.
	*/
	function toNumber(value)
	{
		value = value.replace(/[\$, ]/g, "");
		return parseFloat(value);
	}
	/*---------------------------------------------------------------------------*/

/*****************************************************************************/
/* Custom Editors                                                            */
/*****************************************************************************/
/*---------------------------------------------------------------------------*/
/* Date Type Field Editor                                                    */
/*---------------------------------------------------------------------------*/
/**
	* Base definition of the date type field editor.
	* @param {object} config Configuration information.
	*/
var jsGridDateField = function(config)
{
	jsGrid.Field.call(this, config);
}
/**
	* Object definition of the date type field editor.
 */
jsGridDateField.prototype = new jsGrid.Field(
{
	/**
		* Text alignment for this editor.
	 */
	align: "center",    // general property 'align'
	/**
		* Specific CSS reference for this editor.
	 */
	css: "date-field",  // general property 'css'
	/**
		* Return the editing template of for this instance.
		* @param {object} value Date to edit.
		* @returns {object} jQuery object for use in editing grid value.
		*/
	editTemplate: function(value)
	{
		var grid = this._grid;
		var $result = this._editPicker =
			$("<input>").datepicker().
			datepicker("setDate", getFormattedDate(new Date(value)));
		$result.on("keypress", function(e)
		{
			if(e.which == 13)
			{
				grid.updateItem();
				e.preventDefault;
			}
		});
		$result.on("keyup", function(e)
		{
			if(e.keyCode == 27)
			{
				grid.cancelEdit();
				e.stopPropagation();
			}
		});
		return $result;
	},
	/**
		* Return the final edited value.
		* @returns {string} Final value edited by the user.
		*/
	editValue: function()
	{
		return this._editPicker.datepicker("getDate").toISOString();
	},
	/**
		* Return the editor to be used when inserting a new row.
		* @param {object} value The date to edit.
		* @returns {object} jQuery object allowing user input.
		*/
	insertTemplate: function(value)
	{
		return this._insertPicker =
			$("<input>").datepicker({ defaultDate: new Date() });
	},
	/**
		* Return the final edited value.
		* @returns {string} Final value edited by the user, and ready
		* to be inserted into the date field of a new row.
		*/
	insertValue: function()
	{
		return this._insertPicker.datepicker("getDate").toISOString();
	},
	/**
		* Return the value of the object as a readable, formatted string.
		* @param {object} value Raw value to be converted to date display.
		* @returns {string} Value to be displayed directly in the grid cell.
		*/
	itemTemplate: function(value)
	{
		var result = "";
		if(value)
		{
			result = getFormattedDate(new Date(value));
		}
		return result;
	},
	/**
		* Return the comparison result between two dates.
		* @param {object} date1 First date to compare.
		* @param {object} date2 Next date to compare.
		* @returns {number} The result of the comparison.
		* If > 0, then date1 is larger.
		* If == 0, then date1 and date2 are equal.
		* If < 0, then date2 is larger.
		*/
	sorter: function(date1, date2)
	{
		return new Date(date1) - new Date(date2);
	}
});
/**
	* Create the date editor type within the grid.
 */
jsGrid.fields.date = jsGridDateField;
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Expression Type Field Editor                                              */
/*---------------------------------------------------------------------------*/
var jsGridExpressionField = function(config)
{
	jsGrid.Field.call(this, config);
}
jsGridExpressionField.prototype = new jsGrid.Field(
{
	align: "left",      // General property 'align'
	css: "expression-field", // General property 'css'
	expression: "",     // By default, the expression is unknown.
	itemTemplate: function(value)
	{
		// Value should be set to the record ID.
		// In this version, only implement a very basic expression
		// capability:
		// {FieldName} - Name of a field to display. Multiple field names
		//               are supported, and the same field name can be
		//               specified multiple times.
		// (everything else) - As shown in the string.
		// Example: "{LastName}, {FirstName}"
		//   Display the display name of the person by last name, comma,
		//   then first name.
		var eValue = "";
		var grid = this._grid;
		var item = null;
		var items = grid.data;
		var matches = [];
		var mCount = 0;
		var mIndex = 0;
		var mValue = "";
		var rCount = 0;
		var result = "";
		var rIndex = 0;
		var rName = this.name;
		var rValue = "";
		if(this.expression)
		{
			// Find the record first.
			rCount = items.length;
			// console.log("Expression field: rows count: " + rCount);
			// console.log("Expression field: value: " + value);
			// console.log("Expression field: name:  " + rName);
			for(rIndex = 0; rIndex < rCount; rIndex ++)
			{
				if(items[rIndex][rName] == value)
				{
					// Record found.
					// console.log("Expression field: Record found...");
					item = items[rIndex];
					break;
				}
			}
			if(item)
			{
				eValue = this.expression;
				matches = eValue.match(/\{[0-9A-Za-z]+\}/g);
				mCount = matches.length;
				for(mIndex = 0; mIndex < mCount; mIndex ++)
				{
					mValue = matches[mIndex];
					mValue = mValue.substr(1, mValue.length - 2);
					// console.log("Expression field: Find replacement for " + mValue);
					// Get the replacement value.
					rValue = item[mValue];
					// console.log("Expression field: Use " + rValue);
					// Replace all instances of the field.
					eValue = eValue.replace(
						new RegExp("\\{" + mValue + "\\}", "gm"), rValue);
				}
				result = eValue;
			}
			// Future reference:
			// result = eval("x + 1");
			// if (eval(" var1 == null && var2 != 5")) { ... }
			// result = (new Function("return " + expression)()) { };
		}
		return result;
	}
});
jsGrid.fields.expression = jsGridExpressionField;
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Money Type Field Editor                                                   */
/*---------------------------------------------------------------------------*/
var jsGridMoneyField = function(config)
{
	jsGrid.Field.call(this, config);
}
jsGridMoneyField.prototype = new jsGrid.NumberField(
{
	// myCustomProperty: "foo",  // custom properties can be added
	align: "right",     // general property 'align'
	css: "money-field", // general property 'css'
	itemTemplate: function(value)
	{
		return (value).toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,');  
	}
});
jsGrid.fields.money = jsGridMoneyField;
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* SelectExpression Type Field Editor                                        */
/*---------------------------------------------------------------------------*/
var jsGridSelectExpressionField = function(config)
{
	jsGrid.Field.call(this, config);
}
jsGridSelectExpressionField.prototype = new jsGrid.SelectField(
{
	align: "left",     // general property 'align'
	css: "selectexpression-field", // general property 'css'
	createSelectControl: function()
	{
		var $result = $("<select>");
		var expression = this.expression;
		var items = this.items;
		var valueField = this.valueField;
		var textField = this.textField;
		var rex = this.resolveExpression;
		var selectedIndex = this.selectedIndex;

		$.each(this.items, function(index, item)
		{
			var value = valueField ? item[valueField] : index;
			// var text = textField ? item[textField] : item;

			console.log("CreateSelectControl: value: " +
				value + " / " + rex(value, items, valueField, expression));
			var $option = $("<option>")
				.attr("value", value)
				.text(rex(value, items, valueField, expression))
				.appendTo($result);

			$option.prop("selected", (selectedIndex === index));
		});
		$result.prop("disabled", !!this.readOnly);
		return $result;
	},
	editTemplate: function(value)
	{
		if(!this.editing)
		{
			return this.itemTemplate.apply(this, arguments);
		}
		var grid = this._grid;
		var $result = this.editControl = this.createSelectControl();
		if(value)
		{
			$result.val(value);
		}
		// (value !== undefined) && $result.val(value);
		// console.log("Wiring keypress for selector.");
		$result.on("keypress", function(e)
		{
			if(e.which == 13)
			{
				// console.log("Selector storing changes...");
				grid.updateItem();
				e.preventDefault;
			}
		});
		$result.on("keyup", function(e)
		{
			if(e.keyCode == 27)
			{
				// console.log("Selector cancelling edit...");
				grid.cancelEdit();
				e.stopPropagation();
			}
		});
		return $result;
	},
	expression: "",     // By default, the expression is unknown.
	itemTemplate: function(value)
	{
		return this.resolveExpression(value,
			this.items, this.valueField, this.expression);
	},
	/**
		* Resolve the expression for the specified local ID value.
		* @param {object} value The value designated by the name attribute.
		* @param {object} items Array of items to inspect.
		* @param {string} valueField Name of the value field to scan.
		* @param {string} expression Expression to resolve.
		* @returns {string} Resolved value.
		*/
	resolveExpression: function(value, items, valueField, expression)
	{
		// Value should be set to the record ID.
		// In this version, only implement a very basic expression
		// capability:
		// {FieldName} - Name of a field to display. Multiple field names
		//               are supported, and the same field name can be
		//               specified multiple times.
		// (everything else) - As shown in the string.
		// Example: "{LastName}, {FirstName}"
		//   Display the display name of the person by last name, comma,
		//   then first name.
		var eValue = "";
		var item = null;
		var matches = [];
		var mCount = 0;
		var mIndex = 0;
		var mValue = "";
		var rCount = 0;
		var result = "";
		var rIndex = 0;
		var rName = valueField;
		var rValue = "";
		// console.log("SelectExpression...");
		rCount = items.length;
		// console.log("SelectExpression field: rows count: " + rCount);
		// console.log("SelectExpression field: value:      " + value);
		// console.log("SelectExpression field: key name:   " + rName);
		if(expression)
		{
			// Find the record first.
			for(rIndex = 0; rIndex < rCount; rIndex ++)
			{
				if(items[rIndex][rName] == value)
				{
					// Record found.
					// console.log("SelectExpression field: Record found...");
					item = items[rIndex];
					break;
				}
			}
			if(item)
			{
				eValue = expression;
				matches = eValue.match(/\{[0-9A-Za-z]+\}/g);
				mCount = matches.length;
				for(mIndex = 0; mIndex < mCount; mIndex ++)
				{
					mValue = matches[mIndex];
					mValue = mValue.substr(1, mValue.length - 2);
					// console.log("Expression field: Find replacement for " + mValue);
					// Get the replacement value.
					rValue = item[mValue];
					// console.log("Expression field: Use " + rValue);
					// Replace all instances of the field.
					eValue = eValue.replace(
						new RegExp("\\{" + mValue + "\\}", "gm"), rValue);
				}
				result = eValue;
			}
			// Future reference:
			// result = eval("x + 1");
			// if (eval(" var1 == null && var2 != 5")) { ... }
			// result = (new Function("return " + expression)()) { };
		}
		return result;
	},
	sorter: function(value1, value2)
	{
		// In this context, value1 and value2 both enter with values
		// set to whatever was found in [field.name].
		console.log("SelectExpression lookup...");
		var result = 0;
		var text1 = this.resolveExpression(value1,
			this.items, this.valueField, this.expression);
		var text2 = this.resolveExpression(value2,
			this.items, this.valueField, this.expression);
		if(text1 > text2)
		{
			result = 1;
		}
		else if(text1 < text2)
		{
			result = -1;
		}
		return result;
	}
});
jsGrid.fields.selectexpression = jsGridSelectExpressionField;
/*---------------------------------------------------------------------------*/
/*****************************************************************************/

$(document).ready(function()
{
/*****************************************************************************/
/* View -> View Model Wiring                                                 */
/*****************************************************************************/
/*---------------------------------------------------------------------------*/
/* Column Visibility                                                         */
/*---------------------------------------------------------------------------*/
	$(".columnVisibility .container input[type=checkbox]").change(function()
	{
		// Column Name Pattern: chkcolctlxxxNNNNN
		var columnName = $(this).attr("id").substr(12);
		var fCount = 0;
		var fields = [];
		var fIndex = 0;
		var fItem = null;
		var $grid = null;
		// Parent Name Pattern: colctlNNNNN
		var parentName = $(this).parent().parent().attr("id").substr(6);

		$grid = $("#grd" + parentName);
		console.log("Updating column visibility...");
		console.log("Parent Name: " + parentName);

		// Check fields for match.
		fields = $grid.jsGrid("option", "fields");
		fCount = fields.length;
		console.log("Field count: " + fCount);
		for(fIndex = 0; fIndex < fCount; fIndex ++)
		{
			fItem = fields[fIndex];
			console.log("Checking " + fItem.name)
			if(fItem.name == columnName)
			{
				// Name matches.
				break;
			}
			else if(fItem.title == columnName)
			{
				// Switch to explicit name.
				columnName = fItem.name;
				break;
			}
		}
		$grid.jsGrid("fieldOption", columnName,
			"visible", $(this).prop("checked"));
	});
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Row Filtering                                                             */
/*---------------------------------------------------------------------------*/
	$(".rowFilter input[type=button]").click(function()
	{
		// Button Name Pattern: btnfiltxxxNNNNN.
		var buttonName = $(this).attr("id").substr(10);
		var externalFilters = [];
		var fields = [];
		var $grid = null;
		// Text Name Pattern: txtfiltxxxNNNNN.
		var itemName = "";
		var itemValue = "";
		// Parent Name Pattern: filtNNNN
		var parentName = $(this).parent().attr("id").substr(4);
		var $texts = $(this).parent().find("label input[type=text]");

		$grid = $("#grd" + parentName);
		fields = $grid.jsGrid("option", "fields");
		externalFilters = $grid.jsGrid("option", "externalFilters");
		if(buttonName == "Apply")
		{
			$texts.each(function(tItem)
			{
				itemName = $(this).attr("id").substr(10);
				itemValue = $(this).val();
				externalFilters[itemName] = itemValue;
				// console.log("External Filtering: " +
				//  itemName + "=" + externalFilters[itemName]);
			});
		}
		else if(buttonName == "Clear")
		{
			$texts.each(function(tItem)
			{
				$(this).val("");
				itemName = $(this).attr("id").substr(10);
				externalFilters[itemName] = "";
			});
		}
		// In this context, search is used to reload the data to the UI.
		$grid.jsGrid("search");
	});
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Expanding Sections                                                        */
/*---------------------------------------------------------------------------*/
	$(".tabAccordian").accordion(
		{ collapsible: true, active: false, heightStyle: "content" }
	);
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Tab Pages                                                                 */
/*---------------------------------------------------------------------------*/
	$("#tabs").tabs({
		activate: function(event, ui)
		{
			switch(ui.newPanel.attr('id'))
			{
				case "Accounts":
					// Reload all tables.
					console.log("Tab Selected: Accounts...");
					$("#grdAccounts").jsGrid("fieldOption", "AccountStatus",
						"items", accountsViewModel.accountStates);
					$("#grdAccounts").jsGrid("fieldOption", "BranchID",
						"items", branchesViewModel.branches);
					$("#grdAccounts").jsGrid("fieldOption", "CustomerID",
						"items", customersViewModel.customers);
					$("#grdAccounts").jsGrid("fieldOption", "EmployeeID",
						"items", employeesViewModel.employees);
					$("#grdAccounts").jsGrid("refresh");
					break;
				case "Branches":
					// Refresh display.
					console.log("Tab Selected: Branches...");
					$("#grdBranches").jsGrid("refresh");
					break;
				case "Customers":
					// Refresh display.
					console.log("Tab Selected: Customers...");
					$("#grdCustomers").jsGrid("refresh");
					break;
				case "Employees":
					// Refresh display.
					console.log("Tab Selected: Employees...");
					$("#grdEmployees").jsGrid("refresh");
					break;
			}
		}
	});
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Accounts View                                                             */
/*---------------------------------------------------------------------------*/
$("#grdAccounts").jsGrid(
{
	height: "70vh",
	width: "100%",

	filtering: false,
	editing: true,
	sorting: true,
	paging: true,
	autoload: true,

	pageSize: 4,
	pageButtonCount: 5,

	deleteConfirm: "Do you really want to delete the account?",
	loadMessage: "Loading...",

	controller: accountsViewModel,

	fields: [
		{
			name: "AccountTicket",
			title: "Ticket",
			editing: "false",
			width: 150,
			type: "text"
		},
		{
			name: "AccountStatus",
			title: "Status",
			items: accountsViewModel.accountStates,
			valueField: "StateName",
			textField: "StateName",
			type: "select"
		},
		{
			name: "BalanceAvailable",
			title: "Balance Available",
			type: "money"
		},
		{
			name: "BalancePending",
			title: "Balance Pending",
			type: "money"
		},
		{
			name: "BranchID",
			title: "Branch",
			items: branchesViewModel.branches,
			valueField: "BranchID",
			textField: "Name",
			type: "select"
		},
		{
			name: "CustomerID",
			title: "Customer",
			items: customersViewModel.customers,
			valueField: "CustomerID",
			textField: "Name",
			width: 150,
			type: "select"
		},
		{
			name: "DateClosed",
			title: "Date Closed",
			type: "date"
		},
		{
			name: "DateLastActivity",
			title: "Date Last Activity",
			type: "date"
		},
		{
			name: "DateOpened",
			title: "Date Opened",
			type: "date"
		},
		{
			name: "EmployeeID",
			title: "Employee",
			items: employeesViewModel.employees,
			valueField: "EmployeeID",
			expression: "{LastName}, {FirstName}",
			type: "selectexpression"
		},
		{ type: "control" }
	]
});
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Branches View                                                             */
/*---------------------------------------------------------------------------*/
$("#grdBranches").jsGrid(
{
	height: "70vh",
	width: "100%",

	filtering: false,
	editing: true,
	sorting: true,
	paging: true,
	autoload: true,

	pageSize: 4,
	pageButtonCount: 5,

	deleteConfirm: "Do you really want to delete the branch?",
	loadMessage: "Loading...",

	controller: branchesViewModel,

	fields: [
		{
			name: "BranchTicket",
			title: "Ticket",
			editing: "False",
			width: 150,
			type: "text"
		},
		{
			name: "Name",
			title: "Name",
			editing: "False",
			type: "text"
		},
		{
			name: "Address",
			title: "Address",
			type: "text"
		},
		{
			name: "City",
			title: "City",
			type: "text"
		},
		{
			name: "State",
			title: "State",
			type: "text"
		},
		{
			name: "ZipCode",
			title: "Zip Code",
			type: "text"
		},
		{ type: "control" }
	]
});
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Customers View                                                            */
/*---------------------------------------------------------------------------*/
$("#grdCustomers").jsGrid(
{
	width: "100%",

	filtering: false,
	editing: true,
	sorting: true,
	paging: true,
	autoload: true,

	pageSize: 4,
	pageButtonCount: 5,

	deleteConfirm: "Do you really want to delete the customer?",
	loadMessage: "Loading...",

	controller: customersViewModel,

	fields: [
		{
			name: "CustomerTicket",
			title: "Customer Ticket",
			editing: "False",
			width: 150,
			type: "text"
		},
		{
			name: "Name",
			title: "Name",
			width: 150,
			type: "text"
		},
		{
			name: "Address",
			title: "Address",
			type: "text"
		},
		{
			name: "City",
			title: "City",
			type: "text"
		},
		{
			name: "State",
			title: "State",
			type: "text"
		},
		{
			name: "ZipCode",
			title: "Zip Code",
			type: "text"
		},
		{
			name: "TIN",
			title: "TIN",
			type: "text"
		},
		{ type: "control" }
	]
});
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Employees View                                                            */
/*---------------------------------------------------------------------------*/
$("#grdEmployees").jsGrid(
{
	height: "70vh",
	width: "100%",

	filtering: false,
	editing: true,
	sorting: true,
	paging: true,
	autoload: true,

	pageSize: 4,
	pageButtonCount: 5,

	deleteConfirm: "Do you really want to delete the employee?",
	loadMessage: "Loading...",

	controller: employeesViewModel,

	fields: [
		{
			name: "ExployeeID",
			title: "Display Name",
			expression: "{LastName}, {FirstName}",
			type: "expression"
		},
		{
			name: "EmployeeTicket",
			title: "Ticket",
			editing: "False",
			width: 150,
			type: "text"
		},
		{
			name: "DateStarted",
			title: "Date Started",
			type: "date"
		},
		{
			name: "DateEnded",
			title: "Date Ended",
			type: "date"
		},
		{
			name: "Title",
			title: "Title",
			type: "text"
		},
		{
			name: "TIN",
			title: "TIN",
			type: "text"
		},
		{ type: "control" }
	]
});
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Get Server Data                                                           */
/*---------------------------------------------------------------------------*/
loadData();
/*---------------------------------------------------------------------------*/
/*****************************************************************************/
});

(function()
{
/*****************************************************************************/
/* View Models                                                               */
/*****************************************************************************/
/*---------------------------------------------------------------------------*/
/* Accounts View Model                                                       */
/*---------------------------------------------------------------------------*/
	var accountsViewModel =
	{
		deleteItem: function(deletedItem)
		{
			var accountIndex = $.inArray(deletedItem, this.accounts);
			this.accounts.splice(accountIndex, 1);
		},
		insertItem: function(newItem)
		{
			this.accounts.push(newItem);
		},
		loadData: function(filter)
		{
			console.log("accountsDB.loadData...");
			var cCount = this.accounts.length;
			var cIndex = 0;
			var cItem = null;
			var grid = $("#grdAccounts");
			var result = this.accounts;
			// Remove dashes from tickets.
			for(cIndex = 0; cIndex < cCount; cIndex ++)
			{
				cItem = this.accounts[cIndex];
				cItem.AccountTicket =
					cItem.AccountTicket.replace(/-/g, ' ');
			}
			if(grid.jsGrid("option", "filtering"))
			{
				// Support for internal filtering.
				// This leg used when grid filtering is on.
				result = $.grep(this.accounts, function(account)
				{
					return (!filter.AccountTicket ||
						account.AccountTicket.toLowerCase().
						indexOf(filter.AccountTicket.toLowerCase()) > -1);
				});
			}
			else if(grid.jsGrid("option", "externalFilters"))
			{
				// This leg supports external filtering.
				var externalFilters = grid.jsGrid("option", "externalFilters");
				var fCount = 0;
				var fields = grid.jsGrid("option", "fields");
				var filterName = "";
				var filterValue = "";
				var fIndex = 0;
				var fItem = null;
				var itemTemplate = null;
				var kCount = 0;
				var keys = Object.keys(externalFilters);
				var kIndex = 0;

				kCount = keys.length;
				fCount = fields.length;
				// console.log("Account loadData / key count:   " + kCount);
				// console.log("Account loadData / field count: " + fCount);
				if(kCount > 0 && fCount > 0)
				{
					for(kIndex = 0; kIndex < kCount; kIndex ++)
					{
						filterName = keys[kIndex];
						filterValue = externalFilters[filterName];
						if(filterValue)
						{
							// Filter specified. Get the actual field name.
							for(fIndex = 0; fIndex < fCount; fIndex ++)
							{
								if(filterName == fields[fIndex].Title)
								{
									// Assign the same search to the filter for the base column.
									externalFilters = filterValue;
									break;
								}
							}
						}
					}

					// Indirect filtering example.
					result = $.grep(this.accounts, function(account)
					{
						var fieldItem = null;
						var fieldName = "";
						var fieldType = "";
						var filterFunction = "";
						var filterParts = [];
						var gResult = true;
						var itemValue = null;

						// console.log("Checking " + kCount + " filters...");
						for(kIndex = 0; kIndex < kCount; kIndex ++)
						{
							filterName = keys[kIndex];
							console.log("Filter: " + filterName);
							filterFunction = "";
							filterValue = externalFilters[filterName];
							if(filterName.indexOf(".") > -1)
							{
								// Aggregate, Range, or Function is in use.
								// Currently, only range is supported.
								filterParts = filterName.split(".");
								filterName = filterParts[0];
								filterFunction = filterParts[1].toLowerCase();
							}
							if(filterValue)
							{
								// Filter specified.
								fieldName = "";
								for(fIndex = 0; fIndex < fCount; fIndex ++)
								{
									fieldItem = fields[fIndex];
									if(fieldItem.name == filterName ||
										fieldItem.title == filterName)
									{
										// Field found for filter.
										fieldName = fieldItem.name;
										itemTemplate = fieldItem.itemTemplate;
										if(fieldItem.title == filterName)
										{
											// Copy the filter to the actual field.
											externalFilters[fieldName] = filterValue;
										}
										break;
									}
								}
								if(fieldName)
								{
									itemValue = itemTemplate.call(fieldItem, account[fieldName]);
									// console.log("Checking " + fieldName + " for " + itemValue);
									if(filterFunction)
									{
										// Aggregate, Range, or Function comparison.
										// Currently, only range is supported.
										fieldType = fieldItem.type;
										switch(filterFunction)
										{
											case "max":
												switch(fieldType)
												{
													case "checkbox":
														if(booleanToNumber(toBoolean(itemValue)) >
															booleanToNumber(toBoolean(filterValue)))
														{
															gResult = false;
														}
														break;
													case "date":
														if(new Date(itemValue) > new Date(filterValue))
														{
															gResult = false;
														}
														break;
													case "money":
													case "number":
														if(toNumber(itemValue) > toNumber(filterValue))
														{
															gResult = false;
														}
														break;
													case "select":
													case "selectexpression":
													case "text":
													case "textarea":
														if(itemValue > filterValue)
														{
															gResult = false;
														}
														break;
												}
												break;
											case "min":
												switch(fieldType)
												{
													case "checkbox":
														if(booleanToNumber(toBoolean(itemValue)) <
															booleanToNumber(toBoolean(filterValue)))
														{
															gResult = false;
														}
														break;
													case "date":
														console.log("Min Date Filter (" +
															itemValue + " < " + filterValue + ")?");
														if(new Date(itemValue) < new Date(filterValue))
														{
															gResult = false;
														}
														break;
													case "money":
													case "number":
														console.log("Min Number Filter (" +
														toNumber(itemValue) + " < " + toNumber(filterValue) + ")?");
														if(toNumber(itemValue) < toNumber(filterValue))
														{
															gResult = false;
														}
														break;
													case "select":
													case "selectexpression":
													case "text":
													case "textarea":
														if(itemValue < filterValue)
														{
															gResult = false;
														}
														break;
												}
												break;
										}
										if(!gResult)
										{
											break;
										}
									}
									else
									{
										// Straight value comparison.
										if(itemValue.toLowerCase().
										indexOf(externalFilters[fieldName]) < 0)
										{
											// console.log("Filter non-match in " + fieldName);
											gResult = false;
											break;
										}
									}
								}
							}
							else
							{
								// Make sure that if this filter is empty, any indirectly related
								// filter value is also empty.
								fieldName = "";
								for(fIndex = 0; fIndex < fCount; fIndex ++)
								{
									fieldItem = fields[fIndex];
									if(fieldItem.title == filterName)
									{
										// Field found for filter.
										externalFilters[fieldItem.name] = "";
										break;
									}
								}
							}
						}
						return gResult;
					});
					}
			}
			return result;
		},
		updateItem: function(changedItem)
		{
		}
	};
	window.accountsViewModel = accountsViewModel;
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Branches View Model                                                       */
/*---------------------------------------------------------------------------*/
	var branchesViewModel =
	{
		deleteItem: function(deletedItem)
		{
			var branchIndex = $.inArray(deletedItem, this.branches);
			this.branches.splice(branchIndex, 1);
		},
		insertItem: function(newItem)
		{
			this.branches.push(newItem);
		},
		loadData: function(filter)
		{
			console.log("branchesDB.loadData...");
			var cCount = this.branches.length;
			var cIndex = 0;
			var cItem = null;
			var grid = $("#grdBranches");
			var result = this.branches;
			// Remove dashes from tickets.
			for(cIndex = 0; cIndex < cCount; cIndex ++)
			{
				cItem = this.branches[cIndex];
				cItem.BranchTicket =
					cItem.BranchTicket.replace(/-/g, ' ');
			}
			if(grid.jsGrid("option", "filtering"))
			{
				// Support for internal filtering.
				// This leg used when grid filtering is on.
				result = $.grep(this.branches, function(branch)
				{
					return (!filter.BranchTicket ||
						branch.BranchTicket.toLowerCase().
						indexOf(filter.BranchTicket.toLowerCase()) > -1) &&
						(!filter.Name ||
						branch.Name.toLowerCase().
						indexOf(filter.Name.toLowerCase()) > -1) &&
						(!filter.Address ||
						branch.Address.toLowerCase().
						indexOf(filter.Address.toLowerCase()) > -1) &&
						(!filter.City ||
						branch.City.toLowerCase().
						indexOf(filter.City.toLowerCase()) > -1) &&
						(!filter.State ||
						branch.State.toLowerCase().
						indexOf(filter.State.toLowerCase()) > -1) &&
						(!filter.ZipCode ||
						branch.ZipCode.toLowerCase().
						indexOf(filter.ZipCode.toLowerCase()) > -1);
				});
			}
			else if(grid.jsGrid("option", "externalFilters"))
			{
				// This leg supports external filtering.
				var externalFilters = grid.jsGrid("option", "externalFilters");
				var fCount = 0;
				var fields = grid.jsGrid("option", "fields");
				var filterName = "";
				var filterValue = "";
				var fIndex = 0;
				var fItem = null;
				var itemTemplate = null;
				var kCount = 0;
				var keys = Object.keys(externalFilters);
				var kIndex = 0;

				kCount = keys.length;
				fCount = fields.length;
				// console.log("Branch loadData / key count:   " + kCount);
				// console.log("Branch loadData / field count: " + fCount);
				if(kCount > 0 && fCount > 0)
				{
					for(kIndex = 0; kIndex < kCount; kIndex ++)
					{
						filterName = keys[kIndex];
						filterValue = externalFilters[filterName];
						if(filterValue)
						{
							// Filter specified. Get the actual field name.
							for(fIndex = 0; fIndex < fCount; fIndex ++)
							{
								if(filterName == fields[fIndex].Title)
								{
									// Assign the same search to the filter for the base column.
									externalFilters = filterValue;
									break;
								}
							}
						}
					}

					// Indirect filtering example.
					result = $.grep(this.branches, function(branch)
					{
						var fieldItem = null;
						var fieldName = "";
						var fieldType = "";
						var filterFunction = "";
						var filterParts = [];
						var gResult = true;
						var itemValue = null;

						// console.log("Checking " + kCount + " filters...");
						for(kIndex = 0; kIndex < kCount; kIndex ++)
						{
							filterName = keys[kIndex];
							console.log("Filter: " + filterName);
							filterFunction = "";
							filterValue = externalFilters[filterName];
							if(filterName.indexOf(".") > -1)
							{
								// Aggregate, Range, or Function is in use.
								// Currently, only range is supported.
								filterParts = filterName.split(".");
								filterName = filterParts[0];
								filterFunction = filterParts[1].toLowerCase();
							}
							if(filterValue)
							{
								// Filter specified.
								fieldName = "";
								for(fIndex = 0; fIndex < fCount; fIndex ++)
								{
									fieldItem = fields[fIndex];
									if(fieldItem.name == filterName ||
										fieldItem.title == filterName)
									{
										// Field found for filter.
										fieldName = fieldItem.name;
										itemTemplate = fieldItem.itemTemplate;
										if(fieldItem.title == filterName)
										{
											// Copy the filter to the actual field.
											externalFilters[fieldName] = filterValue;
										}
										break;
									}
								}
								if(fieldName)
								{
									itemValue = itemTemplate.call(fieldItem, account[fieldName]);
									// console.log("Checking " + fieldName + " for " + itemValue);
									if(filterFunction)
									{
										// Aggregate, Range, or Function comparison.
										// Currently, only range is supported.
										fieldType = fieldItem.type;
										switch(filterFunction)
										{
											case "max":
												switch(fieldType)
												{
													case "checkbox":
														if(booleanToNumber(toBoolean(itemValue)) >
															booleanToNumber(toBoolean(filterValue)))
														{
															gResult = false;
														}
														break;
													case "date":
														if(new Date(itemValue) > new Date(filterValue))
														{
															gResult = false;
														}
														break;
													case "money":
													case "number":
														if(toNumber(itemValue) > toNumber(filterValue))
														{
															gResult = false;
														}
														break;
													case "select":
													case "selectexpression":
													case "text":
													case "textarea":
														if(itemValue > filterValue)
														{
															gResult = false;
														}
														break;
												}
												break;
											case "min":
												switch(fieldType)
												{
													case "checkbox":
														if(booleanToNumber(toBoolean(itemValue)) <
															booleanToNumber(toBoolean(filterValue)))
														{
															gResult = false;
														}
														break;
													case "date":
														console.log("Min Date Filter (" +
															itemValue + " < " + filterValue + ")?");
														if(new Date(itemValue) < new Date(filterValue))
														{
															gResult = false;
														}
														break;
													case "money":
													case "number":
														console.log("Min Number Filter (" +
														toNumber(itemValue) + " < " + toNumber(filterValue) + ")?");
														if(toNumber(itemValue) < toNumber(filterValue))
														{
															gResult = false;
														}
														break;
													case "select":
													case "selectexpression":
													case "text":
													case "textarea":
														if(itemValue < filterValue)
														{
															gResult = false;
														}
														break;
												}
												break;
										}
										if(!gResult)
										{
											break;
										}
									}
									else
									{
										// Straight value comparison.
										if(itemValue.toLowerCase().
										indexOf(externalFilters[fieldName]) < 0)
										{
											// console.log("Filter non-match in " + fieldName);
											gResult = false;
											break;
										}
									}
								}
							}
							else
							{
								// Make sure that if this filter is empty, any indirectly related
								// filter value is also empty.
								fieldName = "";
								for(fIndex = 0; fIndex < fCount; fIndex ++)
								{
									fieldItem = fields[fIndex];
									if(fieldItem.title == filterName)
									{
										// Field found for filter.
										externalFilters[fieldItem.name] = "";
										break;
									}
								}
							}
						}
						return gResult;
					});
					}
			}
			return result;
		},
		updateItem: function(changedItem)
		{
		}
	};
	window.branchesViewModel = branchesViewModel;
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Customers View Model                                                      */
/*---------------------------------------------------------------------------*/
	var customersViewModel =
	{
		deleteItem: function(deletedItem)
		{
			var customerIndex = $.inArray(deletedItem, this.customers);
			this.customers.splice(customerIndex, 1);
		},
		insertItem: function(newItem)
		{
			this.customers.push(newItem);
		},
		loadData: function(filter)
		{
			console.log("customersDB.loadData...");
			var cCount = this.customers.length;
			var cIndex = 0;
			var cItem = null;
			var grid = $("#grdCustomers");
			var result = this.customers;
			// Remove dashes from tickets.
			for(cIndex = 0; cIndex < cCount; cIndex ++)
			{
				cItem = this.customers[cIndex];
				cItem.CustomerTicket =
					cItem.CustomerTicket.replace(/-/g, ' ');
			}
			if(grid.jsGrid("option", "filtering"))
			{
				// Support for internal filtering.
				// This leg used when grid filtering is on.
				result = $.grep(this.customers, function(customer)
				{
					return (!filter.CustomerTicket ||
						customer.CustomerTicket.toLowerCase().
						indexOf(filter.CustomerTicket.toLowerCase()) > -1) &&
						(!filter.Name ||
						customer.Name.toLowerCase().
						indexOf(filter.Name.toLowerCase()) > -1) &&
						(!filter.Address ||
						customer.Address.toLowerCase().
						indexOf(filter.Address.toLowerCase()) > -1) &&
						(!filter.City ||
						customer.City.toLowerCase().
						indexOf(filter.City.toLowerCase()) > -1) &&
						(!filter.State ||
						customer.State.toLowerCase().
						indexOf(filter.State.toLowerCase()) > -1) &&
						(!filter.ZipCode ||
						customer.ZipCode.toLowerCase().
						indexOf(filter.ZipCode.toLowerCase()) > -1) &&
						(!filter.TIN ||
						customer.TIN.toLowerCase().
						indexOf(filter.TIN.toLowerCase()) > -1);
				});
			}
			else if(grid.jsGrid("option", "externalFilters"))
			{
				// This leg supports external filtering.
				var externalFilters = grid.jsGrid("option", "externalFilters");
				var fCount = 0;
				var fields = grid.jsGrid("option", "fields");
				var filterName = "";
				var filterValue = "";
				var fIndex = 0;
				var fItem = null;
				var itemTemplate = null;
				var kCount = 0;
				var keys = Object.keys(externalFilters);
				var kIndex = 0;

				kCount = keys.length;
				fCount = fields.length;
				// console.log("Customer loadData / key count:   " + kCount);
				// console.log("Customer loadData / field count: " + fCount);
				if(kCount > 0 && fCount > 0)
				{
					for(kIndex = 0; kIndex < kCount; kIndex ++)
					{
						filterName = keys[kIndex];
						filterValue = externalFilters[filterName];
						if(filterValue)
						{
							// Filter specified. Get the actual field name.
							for(fIndex = 0; fIndex < fCount; fIndex ++)
							{
								if(filterName == fields[fIndex].Title)
								{
									// Assign the same search to the filter for the base column.
									externalFilters = filterValue;
									break;
								}
							}
						}
					}

					// Indirect filtering example.
					result = $.grep(this.customers, function(customer)
					{
						var fieldItem = null;
						var fieldName = "";
						var fieldType = "";
						var filterFunction = "";
						var filterParts = [];
						var gResult = true;
						var itemValue = null;

						// console.log("Checking " + kCount + " filters...");
						for(kIndex = 0; kIndex < kCount; kIndex ++)
						{
							filterName = keys[kIndex];
							console.log("Filter: " + filterName);
							filterFunction = "";
							filterValue = externalFilters[filterName];
							if(filterName.indexOf(".") > -1)
							{
								// Aggregate, Range, or Function is in use.
								// Currently, only range is supported.
								filterParts = filterName.split(".");
								filterName = filterParts[0];
								filterFunction = filterParts[1].toLowerCase();
							}
							if(filterValue)
							{
								// Filter specified.
								fieldName = "";
								for(fIndex = 0; fIndex < fCount; fIndex ++)
								{
									fieldItem = fields[fIndex];
									if(fieldItem.name == filterName ||
										fieldItem.title == filterName)
									{
										// Field found for filter.
										fieldName = fieldItem.name;
										itemTemplate = fieldItem.itemTemplate;
										if(fieldItem.title == filterName)
										{
											// Copy the filter to the actual field.
											externalFilters[fieldName] = filterValue;
										}
										break;
									}
								}
								if(fieldName)
								{
									itemValue = itemTemplate.call(fieldItem, account[fieldName]);
									// console.log("Checking " + fieldName + " for " + itemValue);
									if(filterFunction)
									{
										// Aggregate, Range, or Function comparison.
										// Currently, only range is supported.
										fieldType = fieldItem.type;
										switch(filterFunction)
										{
											case "max":
												switch(fieldType)
												{
													case "checkbox":
														if(booleanToNumber(toBoolean(itemValue)) >
															booleanToNumber(toBoolean(filterValue)))
														{
															gResult = false;
														}
														break;
													case "date":
														if(new Date(itemValue) > new Date(filterValue))
														{
															gResult = false;
														}
														break;
													case "money":
													case "number":
														if(toNumber(itemValue) > toNumber(filterValue))
														{
															gResult = false;
														}
														break;
													case "select":
													case "selectexpression":
													case "text":
													case "textarea":
														if(itemValue > filterValue)
														{
															gResult = false;
														}
														break;
												}
												break;
											case "min":
												switch(fieldType)
												{
													case "checkbox":
														if(booleanToNumber(toBoolean(itemValue)) <
															booleanToNumber(toBoolean(filterValue)))
														{
															gResult = false;
														}
														break;
													case "date":
														console.log("Min Date Filter (" +
															itemValue + " < " + filterValue + ")?");
														if(new Date(itemValue) < new Date(filterValue))
														{
															gResult = false;
														}
														break;
													case "money":
													case "number":
														console.log("Min Number Filter (" +
														toNumber(itemValue) + " < " + toNumber(filterValue) + ")?");
														if(toNumber(itemValue) < toNumber(filterValue))
														{
															gResult = false;
														}
														break;
													case "select":
													case "selectexpression":
													case "text":
													case "textarea":
														if(itemValue < filterValue)
														{
															gResult = false;
														}
														break;
												}
												break;
										}
										if(!gResult)
										{
											break;
										}
									}
									else
									{
										// Straight value comparison.
										if(itemValue.toLowerCase().
										indexOf(externalFilters[fieldName]) < 0)
										{
											// console.log("Filter non-match in " + fieldName);
											gResult = false;
											break;
										}
									}
								}
							}
							else
							{
								// Make sure that if this filter is empty, any indirectly related
								// filter value is also empty.
								fieldName = "";
								for(fIndex = 0; fIndex < fCount; fIndex ++)
								{
									fieldItem = fields[fIndex];
									if(fieldItem.title == filterName)
									{
										// Field found for filter.
										externalFilters[fieldItem.name] = "";
										break;
									}
								}
							}
						}
						return gResult;
					});
					}
			}
			return result;
		},
		updateItem: function(changedItem)
		{
		}
	};
	window.customersViewModel = customersViewModel;
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Employees View Model                                                      */
/*---------------------------------------------------------------------------*/
	var employeesViewModel =
	{
		deleteItem: function(deletedItem)
		{
			var employeeIndex = $.inArray(deletedItem, this.employees);
			this.employees.splice(employeeIndex, 1);
		},
		insertItem: function(newItem)
		{
			this.employees.push(newItem);
		},
		loadData: function(filter)
		{
			console.log("employeesDB.loadData...");
			var cCount = this.employees.length;
			var cIndex = 0;
			var cItem = null;
			var grid = $("#grdEmployees");
			var result = this.employees;
			// Remove dashes from tickets.
			for(cIndex = 0; cIndex < cCount; cIndex ++)
			{
				cItem = this.employees[cIndex];
				cItem.EmployeeTicket =
					cItem.EmployeeTicket.replace(/-/g, ' ');
			}
			if(grid.jsGrid("option", "filtering"))
			{
				// Support for internal filtering.
				// This leg used when grid filtering is on.
				result = $.grep(this.employees, function(employee)
				{
					return (!filter.EmployeeTicket ||
						employee.EmployeeTicket.toLowerCase().
						indexOf(filter.EmployeeTicket.toLowerCase()) > -1) &&
						(!filter.Title ||
						employee.Title.toLowerCase().
						indexOf(filter.Title.toLowerCase()) > -1) &&
						(!filter.TIN ||
						employee.TIN.toLowerCase().
						indexOf(filter.TIN.toLowerCase()) > -1);
				});
			}
			else if(grid.jsGrid("option", "externalFilters"))
			{
				// This leg supports external filtering.
				var externalFilters = grid.jsGrid("option", "externalFilters");
				var fCount = 0;
				var fields = grid.jsGrid("option", "fields");
				var filterName = "";
				var filterValue = "";
				var fIndex = 0;
				var fItem = null;
				var itemTemplate = null;
				var kCount = 0;
				var keys = Object.keys(externalFilters);
				var kIndex = 0;

				kCount = keys.length;
				fCount = fields.length;
				// console.log("Employee loadData / key count:   " + kCount);
				// console.log("Employee loadData / field count: " + fCount);
				if(kCount > 0 && fCount > 0)
				{
					for(kIndex = 0; kIndex < kCount; kIndex ++)
					{
						filterName = keys[kIndex];
						filterValue = externalFilters[filterName];
						if(filterValue)
						{
							// Filter specified. Get the actual field name.
							for(fIndex = 0; fIndex < fCount; fIndex ++)
							{
								if(filterName == fields[fIndex].Title)
								{
									// Assign the same search to the filter for the base column.
									externalFilters = filterValue;
									break;
								}
							}
						}
					}

					// Indirect filtering example.
					result = $.grep(this.employees, function(employee)
					{
						var fieldItem = null;
						var fieldName = "";
						var fieldType = "";
						var filterFunction = "";
						var filterParts = [];
						var gResult = true;
						var itemValue = null;

						// console.log("Checking " + kCount + " filters...");
						for(kIndex = 0; kIndex < kCount; kIndex ++)
						{
							filterName = keys[kIndex];
							console.log("Filter: " + filterName);
							filterFunction = "";
							filterValue = externalFilters[filterName];
							if(filterName.indexOf(".") > -1)
							{
								// Aggregate, Range, or Function is in use.
								// Currently, only range is supported.
								filterParts = filterName.split(".");
								filterName = filterParts[0];
								filterFunction = filterParts[1].toLowerCase();
							}
							if(filterValue)
							{
								// Filter specified.
								fieldName = "";
								for(fIndex = 0; fIndex < fCount; fIndex ++)
								{
									fieldItem = fields[fIndex];
									if(fieldItem.name == filterName ||
										fieldItem.title == filterName)
									{
										// Field found for filter.
										fieldName = fieldItem.name;
										itemTemplate = fieldItem.itemTemplate;
										if(fieldItem.title == filterName)
										{
											// Copy the filter to the actual field.
											externalFilters[fieldName] = filterValue;
										}
										break;
									}
								}
								if(fieldName)
								{
									itemValue = itemTemplate.call(fieldItem, account[fieldName]);
									// console.log("Checking " + fieldName + " for " + itemValue);
									if(filterFunction)
									{
										// Aggregate, Range, or Function comparison.
										// Currently, only range is supported.
										fieldType = fieldItem.type;
										switch(filterFunction)
										{
											case "max":
												switch(fieldType)
												{
													case "checkbox":
														if(booleanToNumber(toBoolean(itemValue)) >
															booleanToNumber(toBoolean(filterValue)))
														{
															gResult = false;
														}
														break;
													case "date":
														if(new Date(itemValue) > new Date(filterValue))
														{
															gResult = false;
														}
														break;
													case "money":
													case "number":
														if(toNumber(itemValue) > toNumber(filterValue))
														{
															gResult = false;
														}
														break;
													case "select":
													case "selectexpression":
													case "text":
													case "textarea":
														if(itemValue > filterValue)
														{
															gResult = false;
														}
														break;
												}
												break;
											case "min":
												switch(fieldType)
												{
													case "checkbox":
														if(booleanToNumber(toBoolean(itemValue)) <
															booleanToNumber(toBoolean(filterValue)))
														{
															gResult = false;
														}
														break;
													case "date":
														console.log("Min Date Filter (" +
															itemValue + " < " + filterValue + ")?");
														if(new Date(itemValue) < new Date(filterValue))
														{
															gResult = false;
														}
														break;
													case "money":
													case "number":
														console.log("Min Number Filter (" +
														toNumber(itemValue) + " < " + toNumber(filterValue) + ")?");
														if(toNumber(itemValue) < toNumber(filterValue))
														{
															gResult = false;
														}
														break;
													case "select":
													case "selectexpression":
													case "text":
													case "textarea":
														if(itemValue < filterValue)
														{
															gResult = false;
														}
														break;
												}
												break;
										}
										if(!gResult)
										{
											break;
										}
									}
									else
									{
										// Straight value comparison.
										if(itemValue.toLowerCase().
										indexOf(externalFilters[fieldName]) < 0)
										{
											// console.log("Filter non-match in " + fieldName);
											gResult = false;
											break;
										}
									}
								}
							}
							else
							{
								// Make sure that if this filter is empty, any indirectly related
								// filter value is also empty.
								fieldName = "";
								for(fIndex = 0; fIndex < fCount; fIndex ++)
								{
									fieldItem = fields[fIndex];
									if(fieldItem.title == filterName)
									{
										// Field found for filter.
										externalFilters[fieldItem.name] = "";
										break;
									}
								}
							}
						}
						return gResult;
					});
					}
			}
			return result;
		},
		updateItem: function(changedItem)
		{
		}
	};
	window.employeesViewModel = employeesViewModel;
/*---------------------------------------------------------------------------*/


/*****************************************************************************/
/* Models                                                                    */
/*****************************************************************************/
/*---------------------------------------------------------------------------*/
/* Account States                                                            */
/*---------------------------------------------------------------------------*/
	accountsViewModel.accountStates =
	[
		{
			"StateName": "Active"
		},
		{
			"StateName": "Closed"
		},
		{
			"StateName": "Pending"
		}
	];
/*---------------------------------------------------------------------------*/

/*---------------------------------------------------------------------------*/
/* Account Models                                                            */
/*---------------------------------------------------------------------------*/
	accountsViewModel.accounts = [];
/*---------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------*/
/* Branch Models                                                             */
/*---------------------------------------------------------------------------*/
	branchesViewModel.branches = [];
/*---------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------*/
/* Customer Models                                                           */
/*---------------------------------------------------------------------------*/
	customersViewModel.customers = [];
/*---------------------------------------------------------------------------*/
/*---------------------------------------------------------------------------*/
/* Employee Models                                                           */
/*---------------------------------------------------------------------------*/
	employeesViewModel.employees = [];
/*---------------------------------------------------------------------------*/
/*****************************************************************************/

}());

