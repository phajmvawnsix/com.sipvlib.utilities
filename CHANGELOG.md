# Changelog

## [2.1.0] - 2026-09-08

Odin Inspector is no longer required or used anywhere in this package. MonoSingleton drops SerializedMonoBehaviour for plain MonoBehaviour. DeepClone/ToBase64String no longer throw without Odin installed -- they now use JsonUtility unconditionally (covers Unity-serializable types; dictionaries not backed by SerializableDictionary, interface-typed fields and polymorphic fields without [SerializeReference] will not round-trip). Adds SerializableDictionary<TKey,TValue> (a Dictionary subclass with a paired key/value inspector drawer), replacing what Odin's serializer previously enabled for dictionary fields.

## [2.0.2] - 2026-09-03

Pin the com.sipvlib.debugging dependency to a semver version (1.1.1) instead of a git URL.
OpenUPM's registry resolves dependency versions as npm-style semver and rejects a package whose
dependencies field contains a raw git URL outright — this fixes "Unable to add package" errors when
installing via the OpenUPM registry.

## [2.0.1] - 2026-09-03

Lower minimum Unity Editor version to 2022.3 LTS (was 6000.3) and add a `repository`
field to `package.json`, both required for OpenUPM registry submission.

`MonoSingleton.Instance` now falls back to the obsolete `FindObjectOfType<T>()` below Unity 2023.1,
since `FindFirstObjectByType<T>()` (introduced 2023.1) doesn't exist on 2022.3. 2023.1+ still uses
`FindFirstObjectByType`, so newer Editors keep its performance benefit over the sorted legacy scan.

## [2.0.0] - 2026-09-01

**Breaking:** `SafeArea` moved to `com.sipvlib.extras.components`. Its namespace changed from
`SiPVLib.Utilities` to `SiPVLib.Extras.Components`; behaviour is unchanged. Add that package and
update the `using` to migrate.

## [1.0.2] - 2026-08-09

Add RectTransform layout utilities.

## [1.0.1] - 2026-07-18

Add SafeArea component.

## [1.0.0] - 2026-07-18

Initial extraction from SiPVLib monolith.
