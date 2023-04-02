namespace Data
{
    public class Diamond
    {
        public readonly byte score;
        public bool isTaken;

        public Diamond(byte score)
        {
            this.score = score;
            isTaken = false;
        }

        public void Take()
        {
            isTaken = true;
        }
    }
}