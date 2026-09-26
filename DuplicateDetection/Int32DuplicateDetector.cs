using System;

namespace DuplicateDetection;

public static class Int32DuplicateDetector
{
    // The word index is `value >> 5` (5 = log2 of the 32 bits per uint word), so
    // BitmapLength * 32 must equal 2^32 to cover the whole int32 domain. That is
    // exactly why this is `1 << 27` and not a rounder-looking number. Shrinking it
    // makes any value above BitmapLength * 32 throw IndexOutOfRangeException.
    private const int BitmapLength = 1 << 27;

    /// <summary>
    /// Determines whether <paramref name="array"/> contains any duplicate values,
    /// using a dense bitmap over the entire <see cref="int"/> domain.
    /// </summary>
    /// <param name="array">The values to inspect. Must not be <see langword="null"/>.</param>
    /// <returns>
    /// <see langword="true"/> if some value appears at least twice; otherwise
    /// <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="array"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// This method allocates a fresh bitmap of <c>BitmapLength</c> <see cref="uint"/>
    /// words on every call. That is 512 MiB, well over the 85 KiB LOH threshold, so
    /// every call allocates on the Large Object Heap and costs O(N + 2^27) time even
    /// for a three-element array. The allocation is deliberate, not a bug: the design
    /// prohibits hash structures, and a dense bitmap over the whole int32 domain is the
    /// chosen trade-off for that constraint. Do not "optimize" it away.
    /// </para>
    /// <para>
    /// Do not hoist the bitmap into a <c>static readonly</c> field to avoid the
    /// allocation. A shared static bitmap is mutated here, so it is not thread-safe,
    /// and this is a public static method with no documented threading contract of
    /// its own. Allocating per call is what makes the method thread-safe as written:
    /// concurrent callers each get their own bitmap and cannot observe or corrupt
    /// each other's state.
    /// </para>
    /// <para>
    /// The <c>array.Length &lt; 2</c> guard is the first statement in the body
    /// precisely so that empty and single-element inputs return before the bitmap is
    /// ever allocated. Do not move it below the allocation.
    /// </para>
    /// </remarks>
    public static bool HasDuplicates(int[] array)
    {
        ArgumentNullException.ThrowIfNull(array);

        if (array.Length < 2)
        {
            return false;
        }

        var seen = new uint[BitmapLength];

        foreach (var element in array)
        {
            var value = unchecked((uint)element);
            var index = (int)(value >> 5);
            var mask = 1u << (int)(value & 31);
            var word = seen[index];

            if ((word & mask) != 0)
            {
                return true;
            }

            seen[index] = word | mask;
        }

        return false;
    }
}
