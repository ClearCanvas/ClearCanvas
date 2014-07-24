The AjaxControlToolkit.dll in this folder is based on June 2012 Release of the toolkit. It contains the fix issue #12374 (Calendar control in ImageServer study search page malfunctions after validation).

Steps to recompile this dll:

1. Download Git (http://git-scm.com/downloads)
2. Clone the repository (https://git01.codeplex.com/ajaxcontroltoolkit)
3. Revert the master branch to the revision of the June 2012 Release (changeset committed by Stephen at June 23 23:19:27)
4. Open the solution in VS 2010. 
5. Fix CalendarBehaviour.pre.js (in Client > MicrosoftAjax.Extended > Calendar):

   - Find "_cell_onclick"
   - Change
   
					case "month":
                        if (target.month == visibleDate.getMonth()) {
                            this._switchMode("days");
                        } else {
                            this._visibleDate = target.date;
                            this._switchMode("days");
                        }
                        break;
                    case "year":
                        if (target.date.getFullYear() == visibleDate.getFullYear()) {
                            this._switchMode("months");
                        } else {
                            this._visibleDate = target.date;
                            this._switchMode("months");
                        }
                        break;
						
		to:		
   
					case "month":
                    	if (target.month == visibleDate.getUTCMonth()) {
                            this._switchMode("days");
                        } else {
                            this._visibleDate = target.date;
                            this._switchMode("days");
                        }
                        break;
                    case "year":
                    	if (this._convertToLocal(target.date).getFullYear() == this._convertToLocal(visibleDate).getFullYear()) {
                            this._switchMode("months");
                        } else {
                            this._visibleDate = target.date;
                            this._switchMode("months");
                        }
                        break;

6. Rebuild the solution