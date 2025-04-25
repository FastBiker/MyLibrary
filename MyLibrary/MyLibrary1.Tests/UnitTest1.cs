using Microsoft.EntityFrameworkCore;
using MyLibrary.Data;
using MyLibrary.Data.Entities;
using MyLibrary.Data.Repositories;
using MyLibrary.Components.InputDataValidation;
using MyLibrary;
using MyLibrary.UserCommunication;

namespace MyLibrary1.Tests
{
    public class Tests
    {
        [Test]
        public void MandatoryInputNotEntered_ShouldEnterInputOneMoreTime()
        {
            // arrange
            //var optionsBuilder = new DbContextOptionsBuilder<MyLibraryDbContext>()
            //.Options;
            //var dbRepository = new DbRepository<Book>(new MyLibraryDbContext(optionsBuilder));
            var _inputDataValidation = new InputDataValidation(new UserCommunication());
            string inf = "Podana wartoœæ jest null / informacja opcjonalana";
            string? input = "";
            // act
            var testInput = _inputDataValidation.InputIsNullOrEmpty(input, inf);

            // assert
            Assert.That(testInput, Is.EqualTo(null));
        }

        [Test]
        public void InputStringNumber_ShouldGetInt()
        {
            // arrange
            var _inputDataValidation = new InputDataValidation(new UserCommunication());
            string input = "67";
            string property = "testProperty";
            int id;
            // act
            id = _inputDataValidation.IntInputValidation(input, property);

            // assert
            Assert.That(id, Is.EqualTo(67));
        }

        [Test]
        public void InputWrongStringNumber_ShouldGetException()
        {
            // arrange
            var _inputDataValidation = new InputDataValidation(new UserCommunication());
            string invalidInput = "23,45";
            string property = "testProperty";
            int id;
            // act & assert
            var ex = Assert.Throws<Exception>(() => _inputDataValidation.IntInputValidation(invalidInput, property));

            Assert.That(ex.Message, Is.EqualTo($"\nPodane dane w 'testProperty' maj¹ niew³aœciw¹ wartoœæ; " +
                "wpisz liczbê ca³kowit¹ wiêksz¹ od '0'!"));
        }

        [Test]
        public void InputSignPlus_ShouldGetBoolTrue()
        {
            // arrange
            var _inputDataValidation = new InputDataValidation(new UserCommunication());
            string testInput = "+";
            string property = "testProperty";
            bool _isTest;
            // act
            _isTest = _inputDataValidation.BoolValidation(testInput, property);

            // assert
            Assert.That(_isTest, Is.EqualTo(true));
        }

        [Test]
        public void InputSignMinus_ShouldGetBoolFalse()
        {
            // arrange
            var _inputDataValidation = new InputDataValidation(new UserCommunication());
            string testInput = "-";
            string property = "testProperty";
            bool _isTest;
            // act
            _isTest = _inputDataValidation.BoolValidation(testInput, property);

            // assert
            Assert.That(_isTest, Is.EqualTo(false));
        }

        [Test]
        public void InputIsEmpty_ShouldGetBoolFalse()
        {
            // arrange
            var _inputDataValidation = new InputDataValidation(new UserCommunication());
            string testInput = "";
            string property = "testProperty";
            bool _isTest;
            // act
            _isTest = _inputDataValidation.BoolValidation(testInput, property);

            // assert
            Assert.That(_isTest, Is.EqualTo(false));
        }

        [Test]
        public void WrongInputBoolValidation_ShouldGetException()
        {
            // arrange
            var _inputDataValidation = new InputDataValidation(new UserCommunication());
            string invalidInput = "test";
            string property = "testProperty";
            // act & assert
            var ex = Assert.Throws<Exception>(() => _inputDataValidation.BoolValidation(invalidInput, property));

            Assert.That(ex.Message, Is.EqualTo($"Podane dane w '{property}' maj¹ niew³aœciw¹ wartoœæ; " +
                "wpisz '+' jeœli jest wypo¿yczona, '-' jeœli nie jest, albo zostaw pole puste"));
        }
    }
}