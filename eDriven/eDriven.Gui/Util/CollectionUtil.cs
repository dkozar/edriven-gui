/*namespace eDriven.Gui.Util
{
    public static class CollectionUtil
    {
        public static int ToggleIndexUp(int currentIndex, int collectionLength, bool loop)
        {
            currentIndex++;
            if (currentIndex >= collectionLength)
            {
                if (loop)
                    currentIndex = 0;
                else
                    currentIndex--;
            }
                
            return currentIndex;
        }

        public static int ToggleIndexDown(int currentIndex, int collectionLength, bool loop)
        {
            currentIndex--;
            if (currentIndex < 0)
                if (loop)
                    currentIndex = collectionLength - 1;
                else
                    currentIndex = 0;

            return currentIndex;
        }
    }
}*/