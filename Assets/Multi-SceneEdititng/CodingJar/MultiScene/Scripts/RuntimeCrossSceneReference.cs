using UnityEngine;
using System.Reflection;

namespace CodingJar.MultiScene
{
	public class ResolveException : System.Exception
	{
		public ResolveException( string message ) : base( message ) {}
	}

	[System.Serializable]
    public struct RuntimeCrossSceneReference
    {
        // From which UniqueObject.PropertyPath?
		[SerializeField]	UniqueObject	_fromObject;
		[SerializeField]	string			_fromField;

        // Which UniqueObject are we referencing?
        [SerializeField]	UniqueObject	_toObject;

		public RuntimeCrossSceneReference( UniqueObject from, string fromField, UniqueObject to )
		{
			_fromObject = from;
			_fromField = fromField;
			_toObject = to;

			_fromObjectCached = null;
			_toObjectCached = null;
		}

		private	Object	_fromObjectCached;
		public	Object	fromObject
		{
			get
			{
				if ( !_fromObjectCached )
					_fromObjectCached = _fromObject.Resolve();

				return _fromObjectCached;
			}
		}

		private	Object	_toObjectCached;
		public	Object	toObject
		{
			get
			{
				if ( !_toObjectCached )
					_toObjectCached = _toObject.Resolve();

				return _toObjectCached;
			}
		}

		public	AmsSceneReference	fromScene
		{
			get { return _fromObject.scene; }
            set { _fromObject.scene = value;  }
		}

		public AmsSceneReference	toScene
		{
			get { return _toObject.scene; }
		}

        public override string ToString()
        {
            return string.Format( "{0}.{1} => {2}", _fromObject, _fromField, _toObject );
        }

		public bool IsSameSource( RuntimeCrossSceneReference other )
		{
			return other._fromObject.Equals(this._fromObject) &&
				other._fromField == this._fromField;
		}

		/// <summary>
		/// Set the cross-scene reference to null.  We want to do this to prevent saves.
		/// </summary>
		public void SetToNull()
		{
			var fromObject = this.fromObject;
			if ( !fromObject )
				throw new ResolveException( string.Format( "Cross-Scene Ref: {0}. Could not Resolve fromObject {1}", this, fromObject ) );

			ResolveInternal( fromObject, null, _fromField, this );
		}

		/// <summary>
		/// Perform a resolve on a cross-scene reference.
		/// This functions throws an exception if it fails.
		/// </summary>
		public void Resolve()
		{
			var fromObject = this.fromObject;
			if ( !fromObject )
				throw new ResolveException( string.Format( "Cross-Scene Ref: {0}. Could not Resolve fromObject {1}", this, fromObject ) );

			Object toObject = this.toObject;
			if ( !toObject )
				throw new ResolveException( string.Format( "Cross-Scene Ref: {0}. Could not Resolve toObject {1}", this, toObject ) );

			ResolveInternal( fromObject, toObject, _fromField, this );
		}

		/// <summary>
		/// Resolve a cross-scene reference piecewise.  This does the heavy lifting of figuring out how to parse the path.
		/// </summary>
		/// <param name="fromObject">The object that is the source of the cross-scene reference</param>
		/// <param name="toObject">The object that the cross-scene reference is referring to</param>
		/// <param name="fromFieldPath">The path of the field that fromObject uses to point to</param>
		/// <param name="debugThis">Debug information about which cross-scene reference this is coming from</param>
		private static void ResolveInternal( System.Object fromObject, Object toObject, string fromFieldPath, RuntimeCrossSceneReference debugThis )
		{
			// Sub-object path is indicated by a dot
			string[] splitPaths = fromFieldPath.Split('.');

			// Since the property is of the form: field1.field2.arrayName,arrayIndex.final_field or simply final_field, we need to chase
			// the property down the rabbit hole starting with the base fromObject and going all the way to final_field.
			for (int i = 0 ; i < splitPaths.Length - 1 ; ++i)
			{
				try
				{
					fromObject = GetObjectFromField( fromObject, splitPaths[i] );
					if ( fromObject == null )
					{
						throw new ResolveException( string.Format("Cross-Scene Ref: {0}. Could not follow path {1} because {2} was null", debugThis, fromFieldPath, splitPaths[i]) );
					}
					else if ( !fromObject.GetType().IsClass )
					{
						throw new ResolveException( string.Format("Cross-Scene Ref: {0}. Could not follow path {1} because {2} was not a class (probably a struct). This is unsupported.", debugThis, fromFieldPath, splitPaths[i]) );
					}
				}
				catch ( System.Exception ex )
				{
					throw new ResolveException( string.Format("Cross-Scene Ref: {0}. {1}", debugThis, ex.Message) );
				}
			}

			// Finally, get the final field.
			FieldInfo field;
			int arrayIndex;
			string fieldName = splitPaths[ splitPaths.Length-1 ];

			if ( !GetFieldFromObject(fromObject, fieldName, out field, out arrayIndex ) )
				throw new ResolveException( string.Format("Cross-Scene Ref: {0}. Could not parse piece of path {1} from {2}", debugThis, fieldName, fromFieldPath) );

			// Now we can finally assign it!
			AssignField( fromObject, toObject, field, arrayIndex );
		}

		/// <summary>
		/// Try to find the field that belongs to fromObject.
		/// </summary>
		/// <param name="fromObject">The source object that has a field named fromField</param>
		/// <param name="fromField">The field name to check for the reference. Can be of the form fieldName,index</param>
		/// <param name="field">The field that represents fromField</param>
		/// <param name="arrayIndex">The array index represented in fromField, or -1 if not an array</param>
		/// <returns>The object referred to by fromObject.fromField</returns>
		private static bool	GetFieldFromObject( System.Object fromObject, string fromField, out FieldInfo field, out int arrayIndex )
		{
			arrayIndex = -1;
			field = null;

			string[] parseField = fromField.Split(',');
			string fieldName = parseField[0];
			
			// Check if it's an array
			if ( parseField.Length > 1 )
			{
				if ( !int.TryParse(parseField[1], out arrayIndex) )
					return false;
			}

			// Find the field.  Need to go through all base classes.
			System.Type objectType = fromObject.GetType();
			while ( objectType != null && field == null )
			{
				field = objectType.GetField( fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy );
				objectType = objectType.BaseType;
			}

			return (field != null);
		}

		/// <summary>
		/// Try to find the object that fromObject.fromField refers to
		/// </summary>
		/// <param name="fromObject">The source object that has a field named fromField</param>
		/// <param name="fromField">The field name to check for the reference</param>
		/// <returns>The object referred to by fromObject.fromField</returns>
		private static System.Object	GetObjectFromField( System.Object fromObject, string fromField )
		{
			int arrayIndex;
			FieldInfo field;

			if ( !GetFieldFromObject( fromObject, fromField, out field, out arrayIndex ) )
				throw new ResolveException( string.Format( "Could not find Field {0}", fromField ) );

			bool isArray = arrayIndex >= 0;
			if ( isArray )
			{
				var list = field.GetValue( fromObject ) as System.Collections.IList;
				if ( list == null )
					throw new ResolveException( string.Format( "Expected collection of elements for property {0} but field type is {1}", field.Name, field.FieldType.Name ) );
				else if ( list.Count <= arrayIndex )
					throw new ResolveException( string.Format( "Expected collection of at least {0} elements from property {1}", arrayIndex+1, field.Name ) );

				// Found it!  Here's the entry.
				return list[arrayIndex];
			}

			return field.GetValue( fromObject );
		}

		/// <summary>
		/// Assigns a specific field fromObject.field[arrayIndex] -> toObject
		/// </summary>
		/// <param name="fromObject">The object to assign from</param>
		/// <param name="toObject">The target object</param>
		/// <param name="field">The field that should be assigned</param>
		/// <param name="arrayIndex">The array index of that field (or negative if it's not an array)</param>
		private static void AssignField( System.Object fromObject, Object toObject, FieldInfo field, int arrayIndex )
		{
			bool isArray = arrayIndex >= 0;
			if ( isArray )
			{
				var list = field.GetValue( fromObject ) as System.Collections.IList;
				if ( list == null )
					throw new ResolveException( string.Format( "Expected collection of elements for property {0} but field type is {1}", field.Name, field.FieldType.Name ) );

				// If it's for a specific index that doesn't yet exist, add it.
				if ( list.Count <= arrayIndex )
				{
					list.Insert( arrayIndex, toObject );
					return;
				}

				// Successful assign!
				try
				{
					list[arrayIndex] = toObject;
				} catch ( System.Exception ex )
				{
					Debug.LogException( ex );
				}
				return;
			}
			else if ( !field.FieldType.IsAssignableFrom( toObject.GetType() ) )
			{
				throw new ResolveException( string.Format( "Field {0} of type {1} is not compatible with {2} of type {3}", field.Name, field.FieldType, toObject, toObject.GetType().Name ) );
			}

			// Successful assign!
			field.SetValue( fromObject, toObject );
		}
	} // struct 
} // namespace 