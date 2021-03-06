﻿// Copyright © 2019 Ivan Kravarščan. All rights reserved. Licensed under the 
// BSD 3-Clause License. See License.txt in the project root for license information.

// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Ikadn.Ikon.Factories.NumericFactory.#NumberFormat")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Scope = "type", Target = "Ikadn.Ikon.Types.IkonArray")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Ikadn.IkadnWriter.Write(System.String)", Scope = "member", Target = "Ikadn.Ikon.Types.IkonBaseObject.#WriteReferences(Ikadn.IkadnWriter)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Ikadn.IkadnWriter.Write(System.String)", Scope = "member", Target = "Ikadn.Ikon.Types.IkonComposite.#DoCompose(Ikadn.IkadnWriter)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Ikadn.IkadnWriter.Write(System.String)", Scope = "member", Target = "Ikadn.Ikon.Types.IkonFloat.#DoCompose(Ikadn.IkadnWriter)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Ikadn.Ikon")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix", Justification = "<Pending>", Scope = "type", Target = "~T:Ikadn.Ikon.Types.IkonComposite")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "AIkonFactory", Scope = "member", Target = "Ikadn.Ikon.IkonParser.#RegisterFactory(Ikadn.IIkadnObjectFactory)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Scope = "member", Target = "Ikadn.Ikon.IkonParser.#RegisterFactory(Ikadn.Ikon.Factories.AIkonFactory)")]
