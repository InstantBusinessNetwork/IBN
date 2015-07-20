using System;
using System.Collections.Generic;
using System.Text;
using ProjectBusinessUtil.Common;
using ProjectBusinessUtil.Services;

namespace ProjectBusinessUtil.Assignment.Contour
{
    /// <summary>
    /// Represents Contour work  factory.
    /// </summary>
    public class ContourFactory : AbstractFactory, IFactoryMethod<StandardContour>, IFactoryMethod<PersonalContour>
    {
        #region Const
        #endregion

        #region Fields
        #endregion

        #region .Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="ContourFactory"/> class.
        /// </summary>
        public ContourFactory()
        {
        }
        #endregion

        #region Properties
        #endregion

        #region Methods
        #endregion

        #region IFactoryMethod<AbstractContour> Members

        /// <summary>
        /// Creates the specified obj.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        StandardContour IFactoryMethod<StandardContour>.Create(object obj)
        {
            ContourTypes contourType = (ContourTypes)Enum.Parse(typeof(ContourTypes), obj.ToString());
            StandardContour retVal = null;
            switch (contourType)
            {
                case ContourTypes.Flat:
                    retVal = new StandardContour(contourType, new AbstractContourBucket[] { new StandardContourBucket(1.0, 1.0) });
                    break;
                case ContourTypes.BackLoaded:
                    retVal = new StandardContour(contourType, new AbstractContourBucket[] 
                          {   // mean is 0.6
					  		new StandardContourBucket(0.1, 0.1), // 10% charge for first 10%
					  		new StandardContourBucket(0.15, 0.1), // 15% charge for next 10%
							new StandardContourBucket(0.25, 0.1), // 25% charge for next 10%
							new StandardContourBucket(0.5, 0.2), // 50% charge for next 20%
							new StandardContourBucket(0.75, 0.2), // 75% charge for next 20%
							new StandardContourBucket(1.0, 0.3) // 100% charge last 30%
                           });
                    break;

                case ContourTypes.FrontLoaded:
                    retVal = new StandardContour(contourType, new AbstractContourBucket[] 
                           {
                           new StandardContourBucket(1.0, 0.3), // 100% charge first 30%
					  		new StandardContourBucket(0.75, 0.2), // 75% charge for next 20%
							new StandardContourBucket(0.5, 0.2), // 50% charge for next 20%
							new StandardContourBucket(0.25, 0.1), // 25% charge for next 10%
							new StandardContourBucket(0.15, 0.1), // 15% charge for next 10%
							new StandardContourBucket(0.1, 0.1) // 10% charge for last 10%	
                           });
                    break;

                case ContourTypes.DoublePeak:
                    retVal = new StandardContour(contourType, new AbstractContourBucket[] 
                           {
                            new StandardContourBucket(0.25, 0.1), // 25% charge first 10%
					  		new StandardContourBucket(0.5, 0.1), // 50% charge for next 10%
							new StandardContourBucket(1.0, 0.1), // 100% charge for next 10%
							new StandardContourBucket(0.5, 0.1), // 50% charge for next 10%			
							new StandardContourBucket(0.25, 0.2), // 25% charge next 20%			
							new StandardContourBucket(0.5, 0.1), // 50% charge for next 10%
							new StandardContourBucket(1.0, 0.1), // 100% charge for next 10%
							new StandardContourBucket(0.5, 0.1), // 50% charge for next 10%			
							new StandardContourBucket(0.25, 0.1), // 25% charge last 10%			
                           });
                    break;
                case ContourTypes.EarlyPeak:
                    retVal = new StandardContour(contourType, new AbstractContourBucket[] 
                           {
                            new StandardContourBucket(0.25, 0.1), // 25% charge first 10%
					  		new StandardContourBucket(0.5, 0.1), // 50% charge for next 10%
							new StandardContourBucket(1.0, 0.2), // 100% charge for next 20%
							new StandardContourBucket(0.75, 0.1), // 75% charge for next 10%			
							new StandardContourBucket(0.5, 0.2), // 50% charge next 20%			
							new StandardContourBucket(0.25, 0.1), // 25% charge for next 10%
							new StandardContourBucket(0.15, 0.1), // 15% charge for next 10%
							new StandardContourBucket(0.1, 0.1), // 10% charge for last 10%		
                           });
                    break;
                case ContourTypes.LatePeak:
                    retVal = new StandardContour(contourType, new AbstractContourBucket[] 
                           {
                            new StandardContourBucket(0.1, 0.1), // 10% charge for first 10%			
					  		new StandardContourBucket(0.15, 0.1), // 15% charge for next 10%
							new StandardContourBucket(0.25, 0.1), // 25% charge for next 10%
							new StandardContourBucket(0.5, 0.2), // 50% charge next 20%			
							new StandardContourBucket(0.75, 0.1), // 75% charge for next 10%			
							new StandardContourBucket(1.0, 0.2), // 100% charge for next 20%
							new StandardContourBucket(0.5, 0.1), // 50% charge for next 10%																		
							new StandardContourBucket(0.25, 0.1) // 25% charge last 10%
                           });
                    break;
                case ContourTypes.Bell:
                    retVal = new StandardContour(contourType, new AbstractContourBucket[] 
                           {
                            new StandardContourBucket(0.1, 0.1), // 10% charge for first 10%			
					  		new StandardContourBucket(0.2, 0.1), // 20% charge for next 10%
							new StandardContourBucket(0.4, 0.1), // 40% charge for next 10%
							new StandardContourBucket(0.8, 0.1), // 80% charge next 10%			
							new StandardContourBucket(1.0, 0.2), // 100% charge for next 20%		
							new StandardContourBucket(0.8, 0.1), // 80% charge next 10%			
							new StandardContourBucket(0.4, 0.1), // 40% charge for next 10%
							new StandardContourBucket(0.2, 0.1), // 20% charge for next 10%			
							new StandardContourBucket(0.1, 0.1) // 10% charge for last 10%		
                           });
                    break;
                case ContourTypes.Turtle:
                    retVal = new StandardContour(contourType, new AbstractContourBucket[] 
                            {
                            new StandardContourBucket(0.25, 0.1), // 25% charge for first 10%			
					  		new StandardContourBucket(0.5, 0.1), // 50% charge for next 10%
							new StandardContourBucket(0.75, 0.1), // 75% charge for next 10%
							new StandardContourBucket(1.0, 0.4), // 100% charge next 40%		
							new StandardContourBucket(0.75, 0.1), // 75% charge for next 10%				
							new StandardContourBucket(0.5, 0.1), // 50% charge for next 10%			
							new StandardContourBucket(0.25, 0.1), // 25% charge for last 10%
                            });
                    break;
                default:
                    retVal = new StandardContour(ContourTypes.Contoured, new AbstractContourBucket[] { });
                    break;
            }

            return retVal;

        }

        #endregion

        #region IFactoryMethod<PersonalContour> Members

        /// <summary>
        /// Creates the personal contour from TimePhased data list.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        PersonalContour IFactoryMethod<PersonalContour>.Create(object obj)
        {
            PersonalContour retVal = null;
            if (obj is KeyValuePair<Assignment, TimePhasedDataType[]>)
            {
                KeyValuePair<Assignment, TimePhasedDataType[]> timePhasedData = (KeyValuePair<Assignment, TimePhasedDataType[]>)obj;
                retVal = new PersonalContour();
                int counter = 0;
                foreach (TimePhasedDataType timePhase in timePhasedData.Value)
                {
                    counter++;
                    Assignment assignment = timePhasedData.Key;
                    long startDuration = assignment.WorkingCalendar.SubstractDates(timePhase.Start, assignment.Start, false);
                    long endDuration = assignment.WorkingCalendar.SubstractDates(timePhase.Finish, assignment.Start, false);

                    if (startDuration == endDuration)
                        continue;

                    double unit = (double)timePhase.Value / (endDuration - startDuration);
                    retVal.InsertInterval(startDuration, endDuration, unit);
                }
            }

            return retVal;
        }

        #endregion
    }
}
