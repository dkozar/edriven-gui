using System;
using System.Collections.Generic;
using eDriven.Gui.Containers;
using UnityEngine;
using Component=eDriven.Gui.Components.Component;

namespace eDriven.Gui.Layout
{
    internal sealed class Flex
    {
        /**
	     *  This function distributes excess space among the flexible children.
	     *  It does so with a view to keep the children's overall size
	     *  close the ratios specified by their percent.
	     *
	     *  Param: spaceForChildren The total space for all children
	     *
	     *  Param: spaceToDistribute The space that needs to be distributed
	     *  among the flexible children.
	     *
	     *  Param: childInfoArray An array of Objects. When this function
	     *  is called, each object should define the following properties:
	     *  - percent: the percentWidth or percentHeight of the child (depending
	     *  on whether we're growing in a horizontal or vertical direction)
	     *  - min: the minimum width (or height) for that child
	     *  - max: the maximum width (or height) for that child
	     *
	     *  Returns: When this function finishes executing, a "size" property
	     *  will be defined for each child object. The size property contains
	     *  the portion of the spaceToDistribute to be distributed to the child.
	     *  Ideally, the sum of all size properties is spaceToDistribute.
	     *  If all the children hit their minWidth/maxWidth/minHeight/maxHeight
	     *  before the space was distributed, then the remaining unused space
	     *  is returned. Otherwise, the return value is zero.
	     */
	    public static float FlexChildrenProportionally(
								    float spaceForChildren,
								    float spaceToDistribute,
								    float totalPercent,
								    List<FlexChildInfo> childInfoArray)
	    {
		    // The algorithm iterivately attempts to break down the space that 
		    // is consumed by "flexible" containers into ratios that are related
		    // to the percentWidth/percentHeight of the participating containers.
    		
		    int numChildren = childInfoArray.Count;
		    float flexConsumed; // space consumed by flexible compontents
		    bool done;
    		
		    // We now do something a little tricky so that we can 
		    // support partial filling of the space. If our total
		    // percent < 100% then we can trim off some space.
		    float unused = spaceToDistribute -
							    (spaceForChildren * totalPercent / 100);
		    if (unused > 0)
			    spaceToDistribute -= unused;

		    // Continue as long as there are some remaining flexible children.
		    // The "done" flag isn't strictly necessary, except that it catches
		    // cases where round-off error causes totalPercent to not exactly
		    // equal zero.
		    do
		    {
			    flexConsumed = 0; // space consumed by flexible compontents
			    done = true; // we are optimistic
    			
			    // Space for flexible children is the total amount of space
			    // available minus the amount of space consumed by non-flexible
			    // components.Divide that space in proportion to the percent
			    // of the child
			    float spacePerPercent = spaceToDistribute / totalPercent;
    			
			    // Attempt to divide out the space using our percent amounts,
			    // if we hit its limit then that control becomes 'non-flexible'
			    // and we run the whole space to distribute calculation again.
			    for (int i = 0; i < numChildren; i++)
			    {
				    FlexChildInfo childInfo = childInfoArray[i];

				    // Set its size in proportion to its percent.
				    float size = childInfo.Percent * spacePerPercent;

				    // If our flexiblity calc say grow/shrink more than we are
				    // allowed, then we grow/shrink whatever we can, remove
				    // ourselves from the array for the next pass, and start
				    // the loop over again so that the space that we weren't
				    // able to consume / release can be re-used by others.
				    if (size < childInfo.Min)
				    {
					    float min = childInfo.Min;
					    childInfo.Size = min;
    					
					    // Move this object to the end of the array
					    // and decrement the length of the array. 
					    // This is slightly expensive, but we don't expect
					    // to hit these min/max limits very often.
					    childInfoArray[i] = childInfoArray[--numChildren];
					    childInfoArray[numChildren] = childInfo;

					    totalPercent -= childInfo.Percent;
					    spaceToDistribute -= min;
					    done = false;
					    break;
				    }

			        if (size > childInfo.Max)
			        {
			            float max = childInfo.Max;
			            childInfo.Size = max;

			            childInfoArray[i] = childInfoArray[--numChildren];
			            childInfoArray[numChildren] = childInfo;

			            totalPercent -= childInfo.Percent;
			            spaceToDistribute -= max;
			            done = false;
			            break;
			        }
			        
                    // All is well, let's carry on...
			        childInfo.Size = size;
			        flexConsumed += size;
			    }
		    } 
		    while (!done);

	        return (float) Math.Max(0, Math.Floor(spaceToDistribute - flexConsumed));
	    }
    }
}
