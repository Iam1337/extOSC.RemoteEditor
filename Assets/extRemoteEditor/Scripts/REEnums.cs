/* Copyright (c) 2018 ExT (V.Sigalkin) */

namespace extRemoteEditor
{
    public enum RECommand
    {
        Clear,              // Clear all generated data.

        GetObjects,         // Get objects.

        GetComponents,      // Get components.

        GetFields,          // Get all fields in component.

        GetValue,           // Get component field.

        SetValue            // Get component value.
    }

    public enum REInvokeStatus
    {
        Complete,           // Request complete.

        Error,              // Request error.

        Cancel              // Cancel request.
    }
}